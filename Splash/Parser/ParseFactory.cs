﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splash.Parser.Basketball;
using Splash.Parser.ESport;
using Splash.Item;
namespace Splash.Parser
{
    class ParseFactory
    {
        public static BaseParser GetParser( SportID sportId,string webName)
        {
            switch(sportId)
            {
                case SportID.SID_BASKETBALL:
                    if (webName == StaticData.webNames[(int)WebID.WID_YABO])
                    {
                        return new BasketballParser_Yabo();
                    }
                    if (webName == StaticData.webNames[(int)WebID.WID_YAYOU])
                    {
                        return new BasketballParser_Yayou();
                    }
                    break;
                case SportID.SID_ESPORT:
                    if (webName == StaticData.webNames[(int)WebID.WID_YABO])
                    {
                        return new ESportParser_Yabo();
                    }
                    else if (webName == StaticData.webNames[(int)WebID.WID_YAYOU])
                    {
                        return new ESportParser_Yayou();
                    }
                    else if (webName == StaticData.webNames[(int)WebID.WID_188])
                    {
                        return new ESportParser_188();
                    }
                    else if (webName == StaticData.webNames[(int)WebID.WID_FANYA])
                    {
                        return new ESportParser_Fanya();
                    }
                    else if (webName == StaticData.webNames[(int)WebID.WID_RAY])
                    {
                        return new ESportParser_Ray();
                    }
                    break;
            }
            return new BaseParser();
        }
    }
}
