using TopoDS;
using gp;
using BRepPrimAPI;
using TopTools;
using BOPAlgo;
using UnitsNet;

namespace OpencascadePart.Elements
{
    public class Tube: BasicElement
    {
        public Length diameter;
        public Length length;
        public Length wallthickness;
        
        
        /// <summary>
        /// generate a hollowed cylinder
        /// </summary>
        /// <param name="myDiameter">diameter</param>
        /// <param name="myLength">length</param>
        /// <param name="myThickness">thickness</param>
        //public HollowedCylinder(double myDiameter, double myLength, double myThickness)
        public Tube(Length diameter, Length length, Length wallthickness) : base()
        {
            this.diameter = diameter;
            this.length = length;
            this.wallthickness = wallthickness;

            Build();
        }


        
        public Tube() : base() { }
        public Tube(string designation, string form, int iD, int iDMaterial, string product, int quantity, Length diameter, Length length, Length wallThickness) 
            : base(designation, form, iD, iDMaterial, product, quantity)
        {
            this.diameter = diameter;
            this.wallthickness = wallThickness;
            this.length = length;

            Build();
        }
        public Tube(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, Length diameter, Length length, Length wallThickness) 
            : this(designation, form, iD, iDMaterial, product, quantity, diameter, length, wallThickness)
        {
            this.GUID = GUID;
        }



        public override void Build()
        {
            // convert values from UnitsNet
            double diam = diameter.Meters;
            double wt = wallthickness.Meters;
            double len = length.Meters;

            // external part
            BRepPrimAPI_MakeCylinder aMakeCylinder = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), diam / 2, len);
            TopoDS_Shape myBody = aMakeCylinder.Shape();

            // internal part
            BRepPrimAPI_MakeCylinder aMakeHollowedPart = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), diam / 2 - wt, len);
            TopoDS_Shape hollowedPart = aMakeHollowedPart.Shape();

            // cut
            BOPAlgo_BOP test = new BOPAlgo_BOP();
            test.AddArgument(myBody);
            TopTools_ListOfShape LS = new TopTools_ListOfShape();
            LS.Append(hollowedPart);
            test.SetTools(LS);
            test.SetRunParallel(true);
            test.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            test.Perform();
            myBody = test.Shape();

            // ______________ triangulation ______________
            SetMyFaces(Triangulation(myBody, 0.7f));
        }
    }
}
