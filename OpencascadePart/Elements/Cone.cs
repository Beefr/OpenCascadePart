using BOPAlgo;
using BRepPrimAPI;
using gp;
using TopoDS;
using TopTools;
using UnitsNet;

namespace OpencascadePart.Elements
{
    public class Cone : BasicElement 
    {
        public Length wallThickness;
        public Length largeEndDiameter;
        public Length smallEndDiameter;
        public Length totalHeight;
        public Angle useless1;
        public Length useless2;
        public Length useless3;
        public Length useless4;
        public Length useless5;


        /// <summary>
        /// create a hollowed cone beheaded
        /// </summary>
        /// <param name="largeEndDiameter">diameter of the base</param>
        /// <param name="totalHeight">height of the cone</param>
        /// <param name="smallEndDiameter">radius of the top</param>
        /// <param name="wallThickness">thickness</param>
        public Cone(Length wallThickness, Length largeEndDiameter, Length smallEndDiameter, Length totalHeight, Angle useless1, Length useless2, Length useless3, Length useless4, Length useless5)
        {

            this.wallThickness = wallThickness;
            this.largeEndDiameter = largeEndDiameter;
            this.smallEndDiameter = smallEndDiameter;
            this.totalHeight = totalHeight;
            this.useless1 = useless1;
            this.useless2 = useless2;
            this.useless3 = useless3;
            this.useless4 = useless4;
            this.useless5 = useless5;

            Build();
        }


        public Cone() : base() { }
        public Cone(string designation, string form, int iD, int iDMaterial, string product, int quantity, Length wallThickness, Length largeEndDiameter, Length smallEndDiameter, Length totalHeight, Angle useless1, Length useless2, Length useless3, Length useless4, Length useless5)
            : base(designation, form, iD, iDMaterial, product, quantity)
        {
            this.wallThickness = wallThickness;
            this.largeEndDiameter = largeEndDiameter;
            this.smallEndDiameter = smallEndDiameter;
            this.totalHeight = totalHeight;
            this.useless1 = useless1;
            this.useless2 = useless2;
            this.useless3 = useless3;
            this.useless4 = useless4;
            this.useless5 = useless5;

            Build();
        }
        public Cone(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, Length wallThickness, Length largeEndDiameter, Length smallEndDiameter, Length totalHeight, Angle useless1, Length useless2, Length useless3, Length useless4, Length useless5)
            : this(designation, form, iD, iDMaterial, product, quantity, wallThickness, largeEndDiameter, smallEndDiameter, totalHeight, useless1, useless2, useless3, useless4, useless5)
        {
            this.GUID = GUID;
        }


        public override void Build()
        {

            // convert values from UnitsNet
            double wallThick = wallThickness.Meters;
            double largeEndDiam = largeEndDiameter.Meters;
            double smallEndDiam = smallEndDiameter.Meters;
            double totalH = totalHeight.Meters;
            double use1 = useless1.Degrees;
            double use2 = useless2.Meters;
            double use3 = useless3.Meters;
            double use4 = useless4.Meters;
            double use5 = useless5.Meters;



            // external part
            BRepPrimAPI_MakeCone aMakeCone = new BRepPrimAPI_MakeCone(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), largeEndDiam / 2, smallEndDiam / 2, totalH);
            TopoDS_Shape myBody = aMakeCone.Shape();
            TopoDS_Solid mySolid = aMakeCone.Solid();

            // internal part
            BRepPrimAPI_MakeCone aMakeHollowedPart = new BRepPrimAPI_MakeCone(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), largeEndDiam / 2 - wallThick, smallEndDiam / 2 - wallThick, totalH);
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
            SetMyFaces(Triangulation(myBody, 0.7f));
        }
    }
}
