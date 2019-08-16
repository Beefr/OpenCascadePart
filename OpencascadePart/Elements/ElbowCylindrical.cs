using BOPAlgo;
using BRepBuilderAPI;
using Geom;
using GeomAPI;
using gp;
using System;
using TColgp;
using TopoDS;
using TopTools;
using UnitsNet;

namespace OpencascadePart.Elements
{
    /// <summary>
    /// coude: BasicElement
    /// </summary>
    public class ElbowCylindrical : ElbowBase
    {
        
        public Length useless1;
        public bool useless2;
        public Length useless3;
        public Length firstEndAdditionalHeight;
        public Length secondEndAdditionalHeight;
        public Angle useless4;
        public Angle useless5;


        /// <summary>
        /// put in MyFaces all the faces that we compute. Note that L1 doesn't exist in BaseElement but i don't figure out on how we can modelize it without L1, there also could be a L2
        /// </summary>
        /// <param name="L1">length between the origin and the most right part of the tube ( same for the top)</param>
        /// <param name="bendingRadius">radius of the pipe</param>
        /// <param name="wallThickness">thickness of the pipe</param>
        /// <param name="bendingAngle"> angle from 0*pi/180 to bendingAngle*pi/180</param>
        /// <param name="n">parameter that modifies the number of triangles</param>
        //public Elbow(double L1, double wallThickness, double bendingRadius, double bendingAngle)
        public ElbowCylindrical(Length wallThickness, Length bendingRadius, Angle bendingAngle, Length useless1, Length useless2, Length useless3, Length firstEndAdditionalHeight, Length secondEndAdditionalHeight, Angle useless4, Angle useless5)
        {

            this.wallThickness = wallThickness;
            this.bendingRadius = bendingRadius;
            this.bendingAngle = bendingAngle;
            this.firstEndAdditionalHeight = firstEndAdditionalHeight;
            this.secondEndAdditionalHeight = secondEndAdditionalHeight;

            Build();
        }


        public ElbowCylindrical() : base() { }
        public ElbowCylindrical(string designation, string form, int iD, int iDMaterial, string product, int quantity, Length wallThickness, Length bendingRadius, Angle bendingAngle, Length useless1, Length useless2, Length useless3, Length firstEndAdditionalHeight, Length secondEndAdditionalHeight, Angle useless4, Angle useless5)
            : base(designation, form, iD, iDMaterial, product, quantity)
        {

            this.wallThickness = wallThickness;
            this.bendingRadius = bendingRadius;
            this.bendingAngle = bendingAngle;
            this.firstEndAdditionalHeight = firstEndAdditionalHeight;
            this.secondEndAdditionalHeight = secondEndAdditionalHeight;

            Build();
        }
        public ElbowCylindrical(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, Length wallThickness, Length bendingRadius, Angle bendingAngle, Length useless1, Length useless2, Length useless3, Length firstEndAdditionalHeight, Length secondEndAdditionalHeight, Angle useless4, Angle useless5)
            : this(designation, form, iD, iDMaterial, product, quantity, wallThickness, bendingRadius, bendingAngle, useless1, useless2, useless3, firstEndAdditionalHeight, secondEndAdditionalHeight, useless4, useless5)
        {
            this.GUID = GUID;
        }


        public override void Build()
        {
            // convert values from UnitsNet
            double wallThick = wallThickness.Meters;
            double bendingRad = bendingRadius.Meters;
            double bendingA = bendingAngle.Degrees;
            double Len1 = firstEndAdditionalHeight.Meters;
            //double Len2 = L2.Meters; // not implemented yet


            // to calculate n
            double min = 20;
            double max = 50;
            double angleLimit = 15;
            double coef = (bendingA - angleLimit) * (max - min) / (180 - angleLimit) > 0 ? (bendingA - angleLimit) * (max - min) / (180 - angleLimit) : 0;
            double n = (int)(min + coef);

            // generate the POINTS for the spline
            double smallShift = Math.PI / 16;
            TColgp_Array1OfPnt array1 = GenerateSpline(Len1, bendingRad, wallThick, 0);
            TColgp_Array1OfPnt array2 = GenerateSpline(Len1, bendingRad, wallThick, smallShift);

            // create the SPLINE with the points
            GeomAPI_PointsToBSpline aSpline1 = new GeomAPI_PointsToBSpline(array1);
            GeomAPI_PointsToBSpline aSpline2 = new GeomAPI_PointsToBSpline(array2);
            Geom_BSplineCurve connectionSpline1 = aSpline1.Curve();
            Geom_BSplineCurve connectionSpline2 = aSpline2.Curve();

            
            // create EXTERNAL shape with spline
            TopoDS_Shape myBody3 = Build(connectionSpline1, bendingA, bendingRad, n, 0);
            
            // create INTERNAL shape with spline
            TopoDS_Shape myBody32 = Build(connectionSpline2, bendingA, bendingRad - wallThick, n, smallShift);



            // ______________ hollowing ______________
            BOPAlgo_BOP cutter = new BOPAlgo_BOP();
            cutter.AddArgument(myBody3);
            TopTools_ListOfShape LSC = new TopTools_ListOfShape();
            LSC.Append(myBody32);
            cutter.SetTools(LSC);
            cutter.SetRunParallel(true);
            cutter.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            cutter.Perform();
            myBody3 = cutter.Shape();




            // ______________ triangulation ______________
            SetMyFaces(Triangulation(myBody3, 0.7f));//*/
        }


        /// <summary>
        /// create the face to be slided
        /// </summary>
        /// <param name="r">the radius of the circular face</param>
        /// <returns>a wire that is our face to be slided</returns>
        public override TopoDS_Wire MakeWire(double r)
        {
            /*gp_Circ2d cir1 = new gp_Circ2d(new gp_Ax2d(new gp_Pnt2d(new gp_XY(0f, 0f)), new gp_Dir2d(1, 0)), r);

            BRepBuilderAPI_MakeEdge2d mee1 = new BRepBuilderAPI_MakeEdge2d(cir1);
            TopoDS_Edge e1 = mee1.Edge();

            BRepBuilderAPI_MakeWire aMakeWire1 = new BRepBuilderAPI_MakeWire(e1);
            TopoDS_Wire W = aMakeWire1.Wire();//*/


            gp_Circ cir1 = new gp_Circ(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(1, 0, 0)), r);
            BRepBuilderAPI_MakeEdge aMakeEdge = new BRepBuilderAPI_MakeEdge(cir1);
            BRepBuilderAPI_MakeWire aMakeWire1 = new BRepBuilderAPI_MakeWire(aMakeEdge.Edge());
            TopoDS_Wire W = aMakeWire1.Wire();

            return W;
        }
    }
}
