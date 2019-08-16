using BOPAlgo;
using BRepBuilderAPI;
using BRepPrimAPI;
using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TopAbs;
using TopoDS;
using TopTools;

namespace OCCTest.Elements
{
    public class Cone : BasicElement 
    {

        /// <summary>
        /// create a hollowed cone beheaded
        /// </summary>
        /// <param name="largeEndDiameter">diameter of the base</param>
        /// <param name="totalHeight">height of the cone</param>
        /// <param name="smallEndDiameter">radius of the top</param>
        /// <param name="wallThickness">thickness</param>
        public Cone(double wallThickness, double largeEndDiameter, double smallEndDiameter, double totalHeight, double useless1, double useless2, double useless3, double useless4, double useless5) {
            
            // external part
            BRepPrimAPI_MakeCone aMakeCone = new BRepPrimAPI_MakeCone(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), largeEndDiameter / 2, smallEndDiameter / 2, totalHeight);
            TopoDS_Shape myBody = aMakeCone.Shape();
            TopoDS_Solid mySolid = aMakeCone.Solid();

            // internal part
            BRepPrimAPI_MakeCone aMakeHollowedPart = new BRepPrimAPI_MakeCone(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), largeEndDiameter / 2- wallThickness, smallEndDiameter / 2- wallThickness, totalHeight);
            TopoDS_Shape hollowedPart = aMakeHollowedPart.Shape();

            // cut
            BOPAlgo_BOP test = new BOPAlgo_BOP();
            test.AddArgument(mySolid);
            TopTools_ListOfShape LS = new TopTools_ListOfShape();
            LS.Append(hollowedPart);
            test.SetTools(LS);
            test.SetRunParallel(true);
            test.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            test.Perform();
            myBody = test.Shape();

            // triangulation
            myFaces = Triangulation(myBody, 0.7f);
        }
    }
}
