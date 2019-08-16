using BRepBuilderAPI;
using BRepPrimAPI;
using GC;
using gp;
using System;
using TopoDS;
using UnitsNet;

namespace OpencascadePart.Elements
{
    /// <summary>
    /// it is supposed to be the dished cover but i didn't have any drawings of the element so i improvised something to give u an example on how u should proceed to build such a shape
    /// knuckle radius bigger than crown radius
    /// </summary>
    public class Caps: BasicElement
    {
        public Length wallThickness;
        public Length diameter;
        public Length crownRadius;
        public Length knucleRadius;
        public Length totalHeight;
        public Length useless1;
        public Pressure useless2;
        public string useless3;
        public bool useless4;


        /// <summary>
        ///  generate a sort of bowl composed with a cylindrical base ( basically a cylinder) and a curved part
        /// </summary>
        /// <param name="totalHeight">total height of the bowl</param>
        /// <param name="thickness">thickness</param>
        /// <param name="heightCylindricalBase">height of the cylinder</param>
        /// <param name="radius1">we need two radiuses to define the curved part</param>
        /// <param name="radius2">we need two radiuses to define the curved part</param>
        //public ConvexTankEnd(double totalHeight, double thickness, double heightCylindricalBase, double radius1, double radius2)
        public Caps(Length wallThickness, Length diameter, Length crownRadius, Length knucleRadius, Length totalHeight, Length useless1, Pressure useless2, string useless3, bool useless4)
        {
            this.wallThickness = wallThickness;
            this.diameter = diameter;
            this.crownRadius = crownRadius;
            this.knucleRadius = knucleRadius;
            this.totalHeight = totalHeight;
            this.useless1 = useless1;
            this.useless2 = useless2;
            this.useless3 = useless3;
            this.useless4 = useless4;

            Build();
        }

        public Caps() : base() { }
        public Caps(string designation, string form, int iD, int iDMaterial, string product, int quantity, Length wallThickness, Length diameter, Length crownRadius, Length knucleRadius, Length totalHeight, Length useless1, Pressure useless2, string useless3, bool useless4) : base(designation, form, iD, iDMaterial, product, quantity)
        {
            this.wallThickness = wallThickness;
            this.diameter = diameter;
            this.crownRadius = crownRadius;
            this.knucleRadius = knucleRadius;
            this.totalHeight = totalHeight;
            this.useless1 = useless1;
            this.useless2 = useless2;
            this.useless3 = useless3;
            this.useless4 = useless4;

            Build();
        }
        public Caps(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, Length wallThickness, Length diameter, Length crownRadius, Length knucleRadius, Length totalHeight, Length useless1, Pressure useless2, string useless3, bool useless4) : this(designation, form, iD, iDMaterial, product, quantity, wallThickness, diameter, crownRadius, knucleRadius, totalHeight, useless1, useless2, useless3, useless4)
        {
            this.GUID = GUID;
        }

        public override void Build() {


            double wallThick = wallThickness.Meters;
            double diam = diameter.Meters;
            double crownRad = crownRadius.Meters;
            double knucleRad = knucleRadius.Meters;
            double totalH = totalHeight.Meters;
            double use1 = useless1.Meters;
            double use2 = useless2.Bars;
            string use3 = useless3;

            //  can't work if P == O
            if (crownRadius == knucleRadius)
                return;

            // _____________curved part_____________
            // on va définir un certain nombre de points
            // certains avec des intersections de cercles, donc va y avoir un peu de calculs...
            gp_Pnt A = new gp_Pnt(wallThick, 0, 0); // point à droite de la base
            gp_Pnt B = new gp_Pnt(0, 0, 0); // point à gauche de la base
            gp_Pnt C = new gp_Pnt(0, totalH, 0); // debut de l'arc exterieur
            gp_Pnt H = new gp_Pnt(wallThick, totalH, 0); // debut de l'arc intérieur
            gp_Pnt P = new gp_Pnt(wallThick + crownRad, totalH, 0); // centre du petit cercle formant l'arc
            gp_Pnt O = new gp_Pnt(wallThick + knucleRad, totalH, 0); // centre du grand cercle formant le capot

            gp_Pnt a1 = new gp_Pnt(crownRad * Math.Cos(3 * Math.PI / 4) + P.X(), crownRad * Math.Sin(3 * Math.PI / 4) + P.Y(), 0); // point de l'arc de cercle intérieur 
            gp_Pnt a2 = new gp_Pnt((crownRad + wallThick) * Math.Cos(3 * Math.PI / 4) + P.X(), (crownRad + wallThick) * Math.Sin(3 * Math.PI / 4) + P.Y(), 0); // point de l'arc de cercle extérieur 
            // pour a3 et a4 c'est compliqué, je crois que je dois choisir un angle au hasard et que potentiellement ça peut tout niquer (si R2 trop petit comparé à R1)
            //gp_Pnt a3 = new gp_Pnt(-radius2 * Math.Cos(3 * Math.PI / 4) + thickness + radius2, radius2* Math.Sin(3 * Math.PI / 4)+ totalHeight - thickness - radius2, 0); // point de l'arc du capot intérieur 
            //gp_Pnt a4 = new gp_Pnt(-(radius2 + thickness) * Math.Cos(3 * Math.PI / 4) + thickness + radius2, (radius2 + thickness) * Math.Sin(3 * Math.PI / 4) + totalHeight - thickness - radius2, 0); // point de l'arc du capot extérieur 
            gp_Pnt E = new gp_Pnt(O.X(), O.Y() + knucleRad+wallThick, 0); // haut du capot
            gp_Pnt F = new gp_Pnt(O.X(), O.Y() + knucleRad, 0); // haut du capot mais côté intérieur
                                                                                                 // maintenant il faut définir les intersections des arcs de cercle et du capot
                                                                                                 // soit l'intersection du cercle de rayon myRadius1(R1) et de centre P (que l'on va abréger P(R1) ) avec O(R2)
                                                                                                 // et également (myThickness=T) P(R1+T) avec O(R2+T)



            gp_Pnt G = new gp_Pnt(P.X(), P.Y() + crownRad, 0); // point de l'arc de cercle intérieur 
            gp_Pnt D = new gp_Pnt(P.X(), P.Y() + crownRad + wallThick, 0); // point de l'arc de cercle extérieur 


            // maintenant qu'on a tous nos points faut les relier ;)
            TopoDS_Edge AB = new BRepBuilderAPI_MakeEdge(A, B).Edge();
            TopoDS_Edge BC = new BRepBuilderAPI_MakeEdge(B, C).Edge();

            GC_MakeArcOfCircle Ca2D = new GC_MakeArcOfCircle(C, a2, D);
            BRepBuilderAPI_MakeEdge meCa2D = new BRepBuilderAPI_MakeEdge(Ca2D.Value());
            TopoDS_Edge CD = meCa2D.Edge();

            TopoDS_Edge DE = new BRepBuilderAPI_MakeEdge(D, E).Edge();
            TopoDS_Edge EF = new BRepBuilderAPI_MakeEdge(E, F).Edge();
            TopoDS_Edge FG = new BRepBuilderAPI_MakeEdge(F, G).Edge();

            GC_MakeArcOfCircle Ga1H = new GC_MakeArcOfCircle(G, a1, H);
            BRepBuilderAPI_MakeEdge meGa1H = new BRepBuilderAPI_MakeEdge(Ga1H.Value());
            TopoDS_Edge GH = meGa1H.Edge();

            TopoDS_Edge HA = new BRepBuilderAPI_MakeEdge(H, A).Edge();


            // creating the wire
            BRepBuilderAPI_MakeWire aMakeWire = new BRepBuilderAPI_MakeWire(AB, BC, CD, DE);
            TopoDS_Wire aWire = aMakeWire.Wire();

            aMakeWire = new BRepBuilderAPI_MakeWire(aWire, EF);
            aWire = aMakeWire.Wire();
            aMakeWire = new BRepBuilderAPI_MakeWire(aWire, FG);
            aWire = aMakeWire.Wire();
            aMakeWire = new BRepBuilderAPI_MakeWire(aWire, GH);
            aWire = aMakeWire.Wire();
            aMakeWire = new BRepBuilderAPI_MakeWire(aWire, HA);
            aWire = aMakeWire.Wire();//*/


            // rotation du wire
            gp_Ax1 Axis = new gp_Ax1(new gp_Pnt(wallThick + knucleRad, 0, 0), new gp_Dir(0, 1, 0)); // origine 0,0,0 avec dir 0,1,0 
            BRepBuilderAPI_MakeFace aMakeFace = new BRepBuilderAPI_MakeFace(aWire);
            TopoDS_Face face = aMakeFace.Face();
            BRepPrimAPI_MakeRevol aMakeRevol = new BRepPrimAPI_MakeRevol(face, Axis, 2 * Math.PI);
            aMakeRevol.Build();
            TopoDS_Shape aRotatedShape = aMakeRevol.Shape();


            // _____________triangulation_____________
            SetMyFaces(Triangulation(aRotatedShape, 0.007f));
            //*/
        }
    }
}
