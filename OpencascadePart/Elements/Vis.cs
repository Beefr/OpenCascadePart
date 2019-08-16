using BOPAlgo;
using BRepBuilderAPI;
using BRepOffsetAPI;
using BRepPrimAPI;
using GCE2d;
using Geom;
using gp;
using System;
using TopoDS;
using TopTools;
using UnitsNet;

namespace OpencascadePart.Elements
{
    public class Vis : BasicElement
    {

        public Length widthBase;
        public Length diameter;
        public Length lengthVis;
        public Length pas;
        public Length heightBase;
        public Length profondeurSillon;
        public Length longueurSillon;


        /// <summary>
        /// create a screw
        /// </summary>
        /// <param name="widthBase">length of the base</param>
        /// <param name="diameter">diameter of the screwed part</param>
        /// <param name="lengthVis">length of the screwed part</param>
        /// <param name="pas">distance travelled by the helicoidal part after 2*pi</param>
        /// <param name="heightBase">height of the base</param>
        /// <param name="profondeurSillon">depth of the screw</param>
        /// <param name="longueurSillon">length of the helicoidal part</param>
        public Vis(Length widthBase, Length diameter, Length lengthVis, Length pas, Length heightBase, Length profondeurSillon, Length longueurSillon)
        {


            this.widthBase = widthBase;
            this.diameter = diameter;
            this.lengthVis = lengthVis;
            this.pas = pas;
            this.heightBase = heightBase;
            this.profondeurSillon = profondeurSillon;
            this.longueurSillon = longueurSillon;

            Build();

        }


        public Vis() : base() { }
        public Vis(string designation, string form, int iD, int iDMaterial, string product, int quantity, Length widthBase, Length diameter, Length lengthVis, Length pas, Length heightBase, Length profondeurSillon, Length longueurSillon) 
            : base(designation, form, iD, iDMaterial, product, quantity)
        {
            this.widthBase = widthBase;
            this.diameter = diameter;
            this.lengthVis = lengthVis;
            this.pas = pas;
            this.heightBase = heightBase;
            this.profondeurSillon = profondeurSillon;
            this.longueurSillon = longueurSillon;

            Build();
        }
        public Vis(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, Length widthBase, Length diameter, Length lengthVis, Length pas, Length heightBase, Length profondeurSillon, Length longueurSillon) 
            : this(designation, form, iD, iDMaterial, product, quantity, widthBase, diameter, lengthVis, pas, heightBase, profondeurSillon, longueurSillon)
        {
            this.GUID = GUID;
        }


        public override void Build()
        {

            // convert values from UnitsNet
            double widthB = widthBase.Meters;
            double diam = diameter.Meters;
            double lengthV = lengthVis.Meters;
            double p = pas.Meters;
            double heightB = heightBase.Meters;
            double profondeurS = profondeurSillon.Meters;
            double longueurS = longueurSillon.Meters;

            // it was hard to parameterize it "correctly", consider taking a screenshot before having fun in modifying it

            double param6 = 2;// pas / (2 * Math.PI); // pas trop grand please
            double param10 = p / (Math.PI / 3); // c'est pour definir le pas
            double param9 = 0;// -param10/2; // position de la vis ?
            double param1 = lengthV - longueurS / 2; // position du centre de la partie helicoidale par rapport au bout pointu de la vis

            double aMinor = 0.05; // epaisseur de la rainure
            double aMajor = (longueurS / p) * Math.PI; // un pi par tour donc 2*pi = 2 tours 

            // Base
            /*Cylinder myBase = new Cylinder(widthBase / 2, heightBase);
            myBase.Translate(new Pnt(0, 0, -heightBase));//*/
            BRepPrimAPI_MakeCylinder aMakeCylinder = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, -heightB), new gp_Dir(0, 0, 1)), widthB / 2, heightB);
            TopoDS_Shape myBase = aMakeCylinder.Shape();

            // helicoidal part
            //Cylinder neck = new Cylinder(diameter / 2, lengthVis);
            BRepPrimAPI_MakeCylinder aMakeCylinder2 = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), diam / 2, lengthV);
            TopoDS_Shape neck = aMakeCylinder2.Shape();

            //threading: define 2d curves
            double depart = param1;
            gp_Pnt2d aPnt = new gp_Pnt2d(2 * Math.PI, param9);
            gp_Dir2d aDir = new gp_Dir2d(2 * Math.PI, param10);
            gp_Ax2d anAx2d = new gp_Ax2d(aPnt, aDir);
            //double aMinor = longueurSillon/5; // epaisseur de la rainure
            Geom2d.Geom2d_Ellipse anEllipse1 = new Geom2d.Geom2d_Ellipse(anAx2d, aMajor, aMinor);
            Geom2d.Geom2d_Ellipse anEllipse2 = new Geom2d.Geom2d_Ellipse(anAx2d, aMajor, aMinor / param6);
            Geom2d.Geom2d_TrimmedCurve anArc1 = new Geom2d.Geom2d_TrimmedCurve(anEllipse1, 0, Math.PI);
            Geom2d.Geom2d_TrimmedCurve anArc2 = new Geom2d.Geom2d_TrimmedCurve(anEllipse2, 0, Math.PI);
            gp_Pnt2d anEllipsePnt1 = anEllipse1.Value(0);
            gp_Pnt2d anEllipsePnt2 = anEllipse1.Value(Math.PI);
            GCE2d_MakeSegment aMakeSegment = new GCE2d_MakeSegment(anEllipsePnt1, anEllipsePnt2);
            Geom2d.Geom2d_TrimmedCurve aSegment = aMakeSegment.Value();
            //threading: build edges and wires
            Geom_CylindricalSurface aCyl1 = new Geom_CylindricalSurface(new gp_Ax3(new gp_Ax2(new gp_Pnt(0, 0, depart), new gp_Dir(0, 0, 1))), (diam / 2) * 0.99);
            Geom_CylindricalSurface aCyl2 = new Geom_CylindricalSurface(new gp_Ax3(new gp_Ax2(new gp_Pnt(0, 0, depart), new gp_Dir(0, 0, 1))), (diam / 2) * (0.99 + profondeurS));
            //
            BRepBuilderAPI_MakeEdge aMakeEdge = new BRepBuilderAPI_MakeEdge(anArc1, aCyl1);
            aMakeEdge.Build();
            TopoDS_Edge anEdge1OnSurf1 = aMakeEdge.Edge();
            //
            aMakeEdge = new BRepBuilderAPI_MakeEdge(aSegment, aCyl1);
            aMakeEdge.Build();
            TopoDS_Edge anEdge2OnSurf1 = aMakeEdge.Edge();
            //
            aMakeEdge = new BRepBuilderAPI_MakeEdge(anArc2, aCyl2);
            aMakeEdge.Build();
            TopoDS_Edge anEdge1OnSurf2 = aMakeEdge.Edge();
            //
            aMakeEdge = new BRepBuilderAPI_MakeEdge(aSegment, aCyl2);
            aMakeEdge.Build();
            TopoDS_Edge anEdge2OnSurf2 = aMakeEdge.Edge();
            //
            BRepBuilderAPI_MakeWire aMakeWire = new BRepBuilderAPI_MakeWire(anEdge1OnSurf1, anEdge2OnSurf1);
            aMakeWire.Build();
            TopoDS_Wire threadingWire1 = aMakeWire.Wire();
            aMakeWire = new BRepBuilderAPI_MakeWire(anEdge1OnSurf2, anEdge2OnSurf2);
            aMakeWire.Build();
            TopoDS_Wire threadingWire2 = aMakeWire.Wire();
            BRepLib.BRepLib.BuildCurves3d(threadingWire1);
            BRepLib.BRepLib.BuildCurves3d(threadingWire2);
            //create threading
            BRepOffsetAPI_ThruSections aTool = new BRepOffsetAPI_ThruSections(true);
            aTool.AddWire(threadingWire1);
            aTool.AddWire(threadingWire2);
            aTool.CheckCompatibility(false);
            TopoDS_Shape myThreading = aTool.Shape();


            // _______fuse_______
            BOPAlgo_BOP adder = new BOPAlgo_BOP();
            adder.AddArgument(myBase);
            TopTools_ListOfShape LSA = new TopTools_ListOfShape();
            LSA.Append(neck);
            adder.SetTools(LSA);
            adder.SetRunParallel(true);
            adder.SetOperation(BOPAlgo_Operation.BOPAlgo_FUSE);
            adder.Perform();
            TopoDS_Shape myBody = adder.Shape();

            // _______fuse_______
            BOPAlgo_BOP adder2 = new BOPAlgo_BOP();
            adder2.AddArgument(myBody);
            TopTools_ListOfShape LSA2 = new TopTools_ListOfShape();
            LSA2.Append(myThreading);
            adder2.SetTools(LSA2);
            adder2.SetRunParallel(true);
            adder2.SetOperation(BOPAlgo_Operation.BOPAlgo_FUSE);
            adder2.Perform();
            myBody = adder2.Shape();

            // _______triangulation_______
            SetMyFaces(Triangulation(myBody, 0.007f));



        }
    }
}
