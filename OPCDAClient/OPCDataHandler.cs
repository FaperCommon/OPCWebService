﻿using Intma.OpcService.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Da.Browsing;

namespace Intma.OPCDAClient
{
    public class OPCDataHandler : IDisposable
    {
        OpcDaGroup _group;
        OpcDaServer _server;

        static int _counter = 1; 
        OpcDaBrowserAuto _browser;
        private int _scanPeriodFromMilliseconds;
        public int ScanPeriodFromMilliseconds { get => _scanPeriodFromMilliseconds; set { _scanPeriodFromMilliseconds = value; _group.UpdateRate = TimeSpan.FromMilliseconds(value); } }
        public Dictionary<string, OPCObject> OPClist { get; set; }


        /// <summary>
        /// Принимается имя OPC сервера
        /// </summary>
        /// <param name="servername"></param>
        public OPCDataHandler(string servername)
        {
            Register(servername);
        }

        public void Register(string servername)
        {
            OPClist = new Dictionary<string, OPCObject>();

            _server = new OpcDaServer(UrlBuilder.Build(servername));
            _server.Connect();

            _group = _server.AddGroup(servername + _counter++);
            _group.IsActive = true;

            _browser = new OpcDaBrowserAuto(_server);

            _group.ValuesChanged += OnGroupValuesChanged;
            ScanPeriodFromMilliseconds = 3000;
        }
        /// <summary>
        /// Совершает подписку на заданные объекты. Возвращает IEnumerable<IOpcUpdatable>, которые будет обновлять
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public List<OPCObject> AddItems(IEnumerable<string> idList)
        {
            // OPClist.Clear();
            List<OpcDaItemDefinition> l = new List<OpcDaItemDefinition>();
            // group.AddItems(idList.Select(a => new OpcDaItemDefinition() { ItemId = a, IsActive = true }).ToList());
            foreach (var id in idList)
            {
                l.Add(new OpcDaItemDefinition
                {
                    ItemId = id,
                    IsActive = true
                });
                var ob = new OPCObject() { ID = id };
                OPClist.Add(id, ob);
            }

            OpcDaItemResult[] results = _group.AddItems(l);

            return OPClist.Values.ToList();
        }

        public void AddItems(IEnumerable<Tag> tags)
        {
            List<OpcDaItemDefinition> l = new List<OpcDaItemDefinition>();
            foreach (var tag in tags)
            {
                AddItem(tag);
                //if (OPClist.Any(a => a.Value.ID == tag.ID))
                //{
                //    if(OPClist[tag.ID].Groups[tag.Group] == null)
                //        OPClist[tag.ID].Groups.Add(tag.Group, tag.Group);

                //    continue;
                //}

                //l.Add(new OpcDaItemDefinition
                //{
                //    ItemId = tag.ID,
                //    IsActive = true
                //});
                //var ob = new OPCObject() { ID = tag.ID, SVGName = tag.SVGName, TagName = tag.TagName };
                //ob.Groups.Add(tag.Group, tag.Group);
                //OPClist.Add(tag.ID, ob);
            }

            //OpcDaItemResult[] results = _group.AddItems(l);
        }

        public void AddItem(Tag tag)
        {
            List<OpcDaItemDefinition> l = new List<OpcDaItemDefinition>();
            OPCObject ob;

            if (OPClist.Any(a => a.Value.ID == tag.ID))
            {
                if (!OPClist[tag.ID].Groups.Any(a => a == tag.Group))
                    OPClist[tag.ID].Groups.Add(tag.Group);
            }
            else { 
                l.Add(new OpcDaItemDefinition
                {
                    ItemId = tag.ID,
                    IsActive = true
                });
                ob = new OPCObject() { ID = tag.ID, TagName = tag.TagName };
                ob.Groups.Add(tag.Group);
                OPClist.Add(tag.ID, ob);

                OpcDaItemResult[] results = _group.AddItems(l);
            }
        }

        /// <summary>
        /// Ручное обновление всех данных
        /// </summary>
        public void Update()
        {
            OpcDaItemValue[] values = _group.Read(_group.Items, OpcDaDataSource.Device);
            foreach (OpcDaItemValue result in values)
            {
                OPClist[result.Item.ItemId].OPCUpdate(result.Quality, result.Value, result.Timestamp.ToString());
            }
        }

        public void Refresh()
        {
            _group.RefreshAsync(OpcDaDataSource.Device);
        }

        void OnGroupValuesChanged(object sender, OpcDaItemValuesChangedEventArgs args)
        {
            foreach (OpcDaItemValue value in args.Values)
            {
                OPClist[value.Item.ItemId].OPCUpdate(value.Quality, value.Value, value.Timestamp.ToString());
            }
        }

        public void WriteValue(OPCObject[] opcObjects)
        {
            // var sel = (from items in group1.Items join opcOb in opcObjects on items.ItemId equals opcOb.ID select items).Distinct().ToList();
            // var sel = _group.Items.Where(a => opcObjects.Any(b => b.ID == a.ItemId)).ToList();
            List<OpcDaItem> sel = new List<OpcDaItem>();
            List<object> values = new List<object>();
            foreach(var opcObject in opcObjects)
            {
                sel.Add(_group.Items.FirstOrDefault(a => a.ItemId == opcObject.ID));
                values.Add(opcObject.Value);
            }
            HRESULT[] results = _group.Write(sel, values.ToArray());
        }

        public void WriteValue(OPCObject opcObject, object value)
        {
            var sel = new List<OpcDaItem>
            {
                _group.Items.FirstOrDefault(a => a.ItemId == opcObject.ID)
            };
            HRESULT[] results = _group.Write(sel, new object[] { value });

        }

        /// <summary>
        /// Получить все дочерние тэги от itemID
        /// </summary>
        /// <param name="itemId">id объекта, null для получения всех тэгов</param>
        /// <returns></returns>
        public IEnumerable<string> BrowseChildren(string itemId = null)
        {
            var itemsList = new List<string>();
            // When itemId is null, root elements will be browsed.
            OpcDaBrowseElement[] elements = _browser.GetElements(itemId);

            foreach (OpcDaBrowseElement element in elements)
            {
                itemsList.Add(element.ItemId);

                if (!element.HasChildren)
                    continue;

                itemsList.AddRange(BrowseChildren(element.ItemId));
            }
            return itemsList;
        }

        public void UnSubscribeAll()
        {
            _group.RemoveItems(_group.Items);
            OPClist.Clear();
        }


        public void UnSubscribe(IEnumerable<string> ids)
        {
            _group.RemoveItems(_group.Items.Where(a => ids.Contains(a.ItemId)).ToList());
            OPClist = OPClist.Where(a => ids.Contains(a.Value.ID)).ToDictionary(x => x.Key, x => x.Value);
        }

        public void Dispose()
        {
            //_group.RemoveItems(_group.Items);

            _server.RemoveGroup(_group);
            _server.Dispose();
        }
    }
}
