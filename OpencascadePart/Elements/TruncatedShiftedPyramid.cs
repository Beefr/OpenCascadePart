using gp;
using System;
using System.Collections.Generic;
using TopAbs;
using UnitsNet;

namespace OpencascadePart.Elements
{
    public class TruncatedShiftedPyramid: BasicElement
    {

        public Length widthBase;
        public Length widthTop;
        public Length myHeight;
        public Angle alpha;



        /// <summary>
        /// create a pyramid beheaded and with a small shift angle
        /// </summary>
        /// <param name="widthBase">width of the base</param>
        /// <param name="widthTop">width of the top</param>
        /// <param name="myHeight">height of the pyramid</param>
        /// <param name="alpha">angle</param>
        public TruncatedShiftedPyramid(Length widthBase, Length widthTop, Length myHeight, Angle alpha)
        {

            this.widthBase = widthBase;
            this.widthTop = widthTop;
            this.myHeight = myHeight;
            this.alpha = alpha;

            Build();
        }


        public TruncatedShiftedPyramid() : base() { }
        public TruncatedShiftedPyramid(string designation, string form, int iD, int iDMaterial, string product, int quantity, Length widthBase, Length widthTop, Length myHeight, Angle alpha) 
            : base(designation, form, iD, iDMaterial, product, quantity)
        {

            this.widthBase = widthBase;
            this.widthTop = widthTop;
            this.myHeight = myHeight;
            this.alpha = alpha;

            Build();
        }
        public TruncatedShiftedPyramid(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, Length widthBase, Length widthTop, Length myHeight, Angle alpha) 
            : this(designation, form, iD, iDMaterial, product, quantity, widthBase, widthTop, myHeight, alpha)
        {
            this.GUID = GUID;
        }


        public override void Build()
        {

            // convert values from UnitsNet
            double widthB = widthBase.Meters;
            double widthT = widthTop.Meters;
            double myH = myHeight.Meters;
            double angle = alpha.Degrees;

            // initialisation
            double rad = angle * 2 * Math.PI / (double)360;
            double e = (widthB - widthT) / 2;
            double x = myH * Math.Tan(rad);
            double L = e + x;


            // LEFT PART
            gp_Pnt aPnt11 = new gp_Pnt(widthB / 2, -widthB / 2, 0);
            gp_Pnt aPnt12 = new gp_Pnt(widthB / 2 - e, -widthB / 2 + L, myH);
            gp_Pnt aPnt13 = new gp_Pnt(-widthB / 2 + e, -widthB / 2 + L, myH);
            gp_Pnt aPnt14 = new gp_Pnt(-widthB / 2, -widthB / 2, 0);


            // RIGHT PART
            gp_Pnt aPnt21 = new gp_Pnt(widthB / 2, widthB / 2, 0);
            gp_Pnt aPnt22 = new gp_Pnt(widthB / 2 - e, -widthB / 2 + L + widthT, myH);
            gp_Pnt aPnt23 = new gp_Pnt(-widthB / 2 + e, -widthB / 2 + L + widthT, myH);
            gp_Pnt aPnt24 = new gp_Pnt(-widthB / 2, widthB / 2, 0);


            //________________________________________________________________

            List<List<gp_Pnt>> faces = new List<List<gp_Pnt>> { new List<gp_Pnt> { aPnt11, aPnt12, aPnt13, aPnt14 }, new List<gp_Pnt> { aPnt21, aPnt22, aPnt23, aPnt24 } };

            // sadly u must know how to orientate faces, the algorithm can't determine it by itself for now
            List<TopAbs_Orientation> orientations = new List<TopAbs_Orientation> { TopAbs_Orientation.TopAbs_REVERSED, TopAbs_Orientation.TopAbs_FORWARD };


            List<Face> tempFaces = new List<Face>();


            Face f1 = new Face(faces[0]);
            f1.orientation = orientations[1];
            tempFaces.Add(f1);

            Face f2 = new Face(faces[1]);
            f2.orientation = orientations[0];
            tempFaces.Add(f2);

            // lateral face
            List<gp_Pnt> allPoints = faces[0];
            allPoints.AddRange(faces[1]);
            Surface surface = new Surface(allPoints, orientations[0]);
            f1 = new Face(surface.f1);
            f2 = new Face(surface.f2);
            tempFaces.AddRange(ComputeFaces(f1, f2, surface.orientation));

            // ordonned pts + triangles
            int count = 0;
            foreach (Face f in tempFaces)
            {
                AddFaces(f.ToTrianglesGraham(true).subFaces);
                count++;
            }
        }
    }
}
