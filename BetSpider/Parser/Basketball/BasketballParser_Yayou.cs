﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Commons;
using BetSpider.Item;
using System.Windows.Forms;

namespace BetSpider.Parser.Basketball
{
    class BasketballParser_Yayou : BasketballParser
    {
        List<int> ids = new List<int>();
        protected override void Init()
        {
            webID = WebID.WID_YAYOU;
            base.Init();
        }
        public override void GrabAndParseHtml()
        {
            int nTryCount = 0;
            string url = "https://b79.lpxin.cn/server/rest/event-tree/prelive/specials?lang=zh&currency=CNY&duration=&marketUID=hdp-ou";
            RequestOptions op = new RequestOptions(url);
            op.Method = "GET";
            string tmpHtml = RequestAction(op);
           
            while (string.IsNullOrEmpty(tmpHtml) && nTryCount < MAX_TRY_COUNT)
            {
                tmpHtml = RequestAction(op);
                nTryCount++;
                Thread.Sleep(1);
            }
            if (nTryCount == MAX_TRY_COUNT)
            {
                MessageBox.Show("抓取失败！");
                tmpHtml = "";
            }
            else
            {
                //MessageBox.Show("OK");
            }

            ParseId(tmpHtml);

            foreach(var id in ids)
            {
                html = GrabFinalHtml(string.Format(urlFormat, id));
                List<BetPlayerItem> items = ParseOneGame(html);
                betItems.AddRange(items);
            }

        }
        private void ParseId(string sec_json)
        {
            ids.Clear();
            int id = 0;
            JArray mObj = JArray.Parse(sec_json);
            foreach (JToken baseJ in mObj)//遍历数组
            {
                string name = baseJ["name"].ToString();
                string path = baseJ["path"].ToString();
                if(path == "‪男子篮球 / 美国 / 美国职业篮球联赛")
                {
                    id = Convert.ToInt32(baseJ["id"]);
                    ids.Add(id);
                }
            }
        }
        public string GrabFinalHtml(string url)
        {
            int nTryCount = 0;
            RequestOptions op = new RequestOptions(url);
            string tmpHtml = RequestAction(op);
            while (string.IsNullOrEmpty(tmpHtml) && nTryCount < MAX_TRY_COUNT)
            {
                tmpHtml = RequestAction(op);
                nTryCount++;
                Thread.Sleep(1);
            }
            if (nTryCount == MAX_TRY_COUNT)
            {
                MessageBox.Show("抓取失败！");
                tmpHtml = "";
            }
            else
            {
               // MessageBox.Show("OK");
            }
            return tmpHtml;
        }
        public override void LoadStaticData()
        {
            
            //EffectiveItem
            int index = 0;
            var eItem = IniUtil.GetString("EffectiveItem", string.Format("I{0}", index), configFile);
            while (!string.IsNullOrEmpty(eItem))
            {
                index++;
                effectItems.Add(eItem);
                eItem = IniUtil.GetString("EffectiveItem", string.Format("I{0}", index), configFile);
            }

            //Player
            index = 0;
            var player = IniUtil.GetString("Players", string.Format("P{0}", index), configFile);
            while (!string.IsNullOrEmpty(player))
            {
                index++;
                playerNames.Add(player);
                player = IniUtil.GetString("Players", string.Format("P{0}", index), configFile);
            }

            //Equal
            var largerPair = IniUtil.GetString("Compare", "C0", configFile).Split('|');
            var smallerPair = IniUtil.GetString("Compare", "C1", configFile).Split('|');
            foreach(var larger in largerPair)
            {
                largers.Add(larger);
            }
            foreach (var smaller in smallerPair)
            {
                smallers.Add(smaller);
            }
        }
        protected override BetCompare GetBetCompare(string parseString)
        {
            foreach (var larger in largers)
            {
                if (parseString.Contains(larger))
                {
                    return BetCompare.Larger;
                }
            }

            foreach (var smaller in smallers)
            {
                if (parseString.Contains(smaller))
                {
                    return BetCompare.Smaller;
                }
            }
            return BetCompare.Smaller;
        }
        protected override double GetBetValue(string parseString)
        {
            Match match = Regex.Match(parseString, @"(.*)[大|小](.*)");
            string strValue = match.Groups[2].ToString().Trim();
            double value = Convert.ToDouble(strValue);
            return value;
        }
        protected override int GetPlayerIndex(string parseString)
        {
            Match match = Regex.Match(parseString, @"(.*)[大|小]\w");
            string playerName = match.Groups[1].ToString().Trim();
            if (playerNames.Contains(playerName))
            {
                return playerNames.IndexOf(playerName);
            }
            playerNames.Add(playerName);
            IniUtil.WriteString("Players", string.Format("P{0}", playerNames.Count - 1), playerName, "D:\\Yayou.ini");
            return playerNames.Count - 1;
        }
        public List<BetPlayerItem> ParseOneGame(string data)
        {
            try
            {
                JObject mObj = JObject.Parse(data);
                int id = Convert.ToInt32(mObj["live"]);
                string name = mObj["name"].ToString();
                var teams = name.Split('-');
                int sex = Convert.ToInt32(mObj["ordNo"]);
                JToken mChild = mObj["children"];

                int itemIndex = 0;
                betItems.Clear();
                foreach (JToken baseJ in mChild)//遍历数组
                {
                    var live = baseJ["live"];
                    var itemName = baseJ["name"];
                    IniUtil.WriteString("Items", string.Format("I{0}", itemIndex), itemName.ToString(), configFile);
                    for (int i = 0; i < effectItems.Count; i++)
                    {
                        if (itemName.ToString() == effectItems[i])
                        {
                            var effectItem = baseJ["children"];
                            foreach (JToken eI in effectItem)//遍历数组
                            {
                                var playerNameToken = eI["name"];
                                BetPlayerItem item = new BetPlayerItem();
                                item.compare = GetBetCompare(playerNameToken.ToString());
                                item.playerIndex = GetPlayerIndex(playerNameToken.ToString());
                                item.playerName = playerNames[item.playerIndex];
                                item.value = GetBetValue(playerNameToken.ToString());
                                item.odds = Convert.ToDouble(eI["odds"]);
                                item.itemIndex = i;
                                betItems.Add(item);
                            }
                        }
                    }
                    itemIndex++;
                }
                return betItems;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }
    }
}
