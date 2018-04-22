using System.Collections.Generic;

namespace PGA.Model.DAL
{
    public partial class EntitiesModel
    {
     
        IList<string> Layerstates = new List<string>();

        public void LoadLayerStates()
        {
            Layerstates.Add("BRIDGE,OBR,S-BRIDGE");
            Layerstates.Add("BUILDING,OBD,S-BUILDING");
            Layerstates.Add("BUNKER,OST,S-BUNKER");
            Layerstates.Add("BUSH OUTLINE,OBO,S-BUSH");
            Layerstates.Add("CARTPATH,OCA,S-CARTPATH");
            Layerstates.Add("COLLAR,OCO,S-COLLAR");
            Layerstates.Add("DIRT OUTLINE,ODO,S-DIRT-OUTLINE");
            Layerstates.Add("FAIRWAY,OFW,S-FAIRWAY");
            Layerstates.Add("GREEN,OGR,S-GREEN");
            Layerstates.Add("GREENSIDE BUNKER,OGS,S-GREENSIDE-BUNKER");
            Layerstates.Add("INTERMEDIATE ROUGH,OIR,S-INTERMEDIATE-ROUGH");
            Layerstates.Add("LANDSCAPING,OLN,S-LANDSCAPING");
            Layerstates.Add("NATIVE AREA / SAWGRASS,ONA,S-NATIVE-AREA");
            Layerstates.Add("OTHER,OTH,S-OTHER");
            Layerstates.Add("PATH,OPT,S-PATH");
            Layerstates.Add("ROCK OUTLINE,ORK,S-ROCK-OUTLINE");
            Layerstates.Add("ROUGH OUTLINE,ORO,S-ROUGH-OUTLINE");
            Layerstates.Add("STEPS,ORK,S-STEPS");
            Layerstates.Add("TEE BOX,OTB,S-TEE-BOX");
            Layerstates.Add("TREE OUTLINE,OTO,S-TREE-OUTLINE");
            Layerstates.Add("WALK STRIP,OWS,S-WALK-STRIP");
            Layerstates.Add("WALL,OWL,S-WALL");
            Layerstates.Add("WATER,OWA,S-WATER");
            Layerstates.Add("WATER DROP,OWD,S-WATER-DROP");
            
        }

    }
}
