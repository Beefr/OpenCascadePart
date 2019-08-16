using BRepPrimAPI;
using gp;
using TopoDS;

namespace OpencascadePart.Elements
{
    /// <summary>
    /// useless, can't be generated on the client, consider deleting it
    /// </summary>
    public class Cylinder : BasicElement
    {
        /// <summary>
        /// generate a cylinder
        /// </summary>
        /// <param name="myDiameter">diameter</param>
        /// <param name="myHeight">height</param>
        public Cylinder(double myDiameter, double myHeight) {
            BRepPrimAPI_MakeCylinder aMakeCylinder = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), myDiameter/2, myHeight);
            TopoDS_Shape myBody = aMakeCylinder.Shape();


            // ______________ triangulation ______________
            SetMyFaces(Triangulation(myBody, 0.7f));

        }

        public override void Build() { }

    }
}
