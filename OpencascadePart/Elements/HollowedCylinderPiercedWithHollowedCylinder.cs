using BOPAlgo;
using BRepPrimAPI;
using gp;
using System;
using TopoDS;
using TopTools;
using UnitsNet;

namespace OpencascadePart.Elements
{
    public class HollowedCylinderPiercedWithHollowedCylinder: BasicElement
    {
        public Length myRadius;
        public Length myHeight;
        public Length myThickness;
        public Length myRadius2;
        public Length myHeight2;
        public Length myThickness2;
        public Length pierceHeight;

        /// <summary>
        /// create a hollowed cylinder that is pierced with another cylinder
        /// </summary>
        /// <param name="myRadius">radius of the big cylinder</param>
        /// <param name="myHeight">height of the big cylinder</param>
        /// <param name="myThickness">thickness of the big cylinder</param>
        /// <param name="myRadius2">radius of the small cylinder</param>
        /// <param name="myHeight2">height of the small cylinder</param>
        /// <param name="myThickness2">thickness of the small cylinder</param>
        /// <param name="pierceHeight">height on which we pierce</param>
        public HollowedCylinderPiercedWithHollowedCylinder(Length myRadius, Length myHeight, Length myThickness, Length myRadius2, Length myHeight2, Length myThickness2, Length pierceHeight)
        {

            this.myRadius = myRadius;
            this.myHeight = myHeight;
            this.myThickness = myThickness;
            this.myRadius2 = myRadius2;
            this.myHeight2 = myHeight2;
            this.myThickness2 = myThickness2;
            this.pierceHeight = pierceHeight;

            Build();

        }


        public HollowedCylinderPiercedWithHollowedCylinder() : base() { }
        public HollowedCylinderPiercedWithHollowedCylinder(string designation, string form, int iD, int iDMaterial, string product, int quantity, Length myRadius, Length myHeight, Length myThickness, Length myRadius2, Length myHeight2, Length myThickness2, Length pierceHeight)
            : base(designation, form, iD, iDMaterial, product, quantity)
        {

            this.myRadius = myRadius;
            this.myHeight = myHeight;
            this.myThickness = myThickness;
            this.myRadius2 = myRadius2;
            this.myHeight2 = myHeight2;
            this.myThickness2 = myThickness2;
            this.pierceHeight = pierceHeight;

            Build();
        }
        public HollowedCylinderPiercedWithHollowedCylinder(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, Length myRadius, Length myHeight, Length myThickness, Length myRadius2, Length myHeight2, Length myThickness2, Length pierceHeight)
            : this(designation, form, iD, iDMaterial, product, quantity, myRadius, myHeight, myThickness, myRadius2, myHeight2, myThickness2, pierceHeight)
        {
            this.GUID = GUID;
        }

        public override void Build()
        {

            // convert values from UnitsNet
            double myRad = myRadius.Meters;
            double myH = myHeight.Meters;
            double myThick = myThickness.Meters;
            double myRad2 = myRadius2.Meters;
            double myH2 = myHeight2.Meters;
            double myThick2 = myThickness2.Meters;
            double pierceH = pierceHeight.Meters;


            //  _______first cylinder (the big one) _______
            BRepPrimAPI_MakeCylinder aMakeCylinder = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), myRad, myH);
            TopoDS_Shape myBody = aMakeCylinder.Shape();
            TopoDS_Solid mySolid = aMakeCylinder.Solid();


            // inner cylinder of the bigger cylinder to be hollowed
            BRepPrimAPI_MakeCylinder aMakeHollowedPart = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), myRad - myThick, myH);
            TopoDS_Shape hollowedPart = aMakeHollowedPart.Shape();

            // hollowing the bigger cylinder
            BOPAlgo_BOP test = new BOPAlgo_BOP();
            test.AddArgument(mySolid);
            TopTools_ListOfShape LS = new TopTools_ListOfShape();
            LS.Append(hollowedPart);
            test.SetTools(LS);
            test.SetRunParallel(true);
            test.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            test.Perform();
            myBody = test.Shape();


            // _______second cylinder (the smaller one) _______
            BRepPrimAPI_MakeCylinder aMakeCylinder2 = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, -myThick + Math.Sqrt(myRad * myRad - myRad2 * myRad2), pierceH), new gp_Dir(0, 1, 0)), myRad2, myH2);
            TopoDS_Shape myBody2 = aMakeCylinder2.Shape();
            TopoDS_Solid mySolid2 = aMakeCylinder2.Solid();


            // inner cylinder of the smaller cylinder to be hollowed
            BRepPrimAPI_MakeCylinder aMakeHollowedPart2 = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, -myThick + Math.Sqrt(myRad * myRad - myRad2 * myRad2), pierceH), new gp_Dir(0, 1, 0)), myRad2 - myThick2, myH2);
            TopoDS_Shape hollowedPart2 = aMakeHollowedPart2.Shape();


            // smaller cylinder hollowed
            BOPAlgo_BOP test2 = new BOPAlgo_BOP();
            test2.AddArgument(mySolid2);
            TopTools_ListOfShape LS2 = new TopTools_ListOfShape();
            LS2.Append(hollowedPart2);
            test2.SetTools(LS2);
            test2.SetRunParallel(true);
            test2.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            test2.Perform();
            TopoDS_Shape hollowedSmall = test2.Shape();

            // piercing
            BOPAlgo_BOP piercer = new BOPAlgo_BOP();
            piercer.AddArgument(myBody);
            TopTools_ListOfShape LSP = new TopTools_ListOfShape();
            LSP.Append(hollowedPart2);
            piercer.SetTools(LSP);
            piercer.SetRunParallel(true);
            piercer.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            piercer.Perform();
            myBody = piercer.Shape();


            // adding the tube
            BOPAlgo_BOP adder = new BOPAlgo_BOP();
            adder.AddArgument(myBody);
            TopTools_ListOfShape LSA = new TopTools_ListOfShape();
            LSA.Append(hollowedSmall);
            adder.SetTools(LSA);
            adder.SetRunParallel(true);
            adder.SetOperation(BOPAlgo_Operation.BOPAlgo_FUSE);
            adder.Perform();
            myBody = adder.Shape();//*/



            // _______triangulation_______
            SetMyFaces(Triangulation(myBody, 0.007f));
        }
    }
}
