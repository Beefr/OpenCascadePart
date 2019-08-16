using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TopAbs;
using UnitsNet;

namespace OpencascadePart.Elements
{
    public class PipeRectangular: BasicElement
    {
        public Length wallThickness;
        public Length pipeLength;
        public Length rectangleLength;
        public Length rectangleWidth;

        /// <summary>
        /// generate a tube with a cubic shape
        /// </summary>
        /// <param name="myWidth">width</param>
        /// <param name="myHeight">height</param>
        /// <param name="myThickness">thickness</param>
        /// <param name="myLength">length</param>
        //public CubicTube(double myWidth, double myHeight, double myThickness, double myLength)
        public PipeRectangular(Length wallThickness, Length pipeLength, Length rectangleLength, Length rectangleWidth)
        {

            this.wallThickness = wallThickness;
            this.pipeLength = pipeLength;
            this.rectangleLength = rectangleLength;
            this.rectangleWidth = rectangleWidth;


            Build();
        }



        public PipeRectangular() : base() { }
        public PipeRectangular(string designation, string form, int iD, int iDMaterial, string product, int quantity, Length wallThickness, Length pipeLength, Length rectangleLength, Length rectangleWidth)
            : base(designation, form, iD, iDMaterial, product, quantity)
        {

            this.wallThickness = wallThickness;
            this.pipeLength = pipeLength;
            this.rectangleLength = rectangleLength;
            this.rectangleWidth = rectangleWidth;

            Build();
        }
        public PipeRectangular(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, Length wallThickness, Length pipeLength, Length rectangleLength, Length rectangleWidth)
            : this(designation, form, iD, iDMaterial, product, quantity, wallThickness, pipeLength, rectangleLength, rectangleWidth)
        {
            this.GUID = GUID;
        }
        public PipeRectangular(string designation, string form, System.Guid GUID, int iD, int iDMaterial, string product, int quantity, string shape, Length wallThickness, Length pipeLength, Length rectangleLength, Length rectangleWidth)
            : this(designation, form, iD, iDMaterial, product, quantity, wallThickness, pipeLength, rectangleLength, rectangleWidth)
        {
            this.GUID = GUID;
        }

        public override void Build()
        {

            // convert values from UnitsNet
            double wallThick = wallThickness.Meters;
            double pipeL = pipeLength.Meters;
            double rectangleL = rectangleLength.Meters;
            double rectangleW = rectangleWidth.Meters;

            // note that u could have achieved the same by creating a wire, then make it slide on a spline to create a pipe, then cut external and internal shapes and finally triangulate it (example in the elbow file)

            // inferior part
            // base part ext
            gp_Pnt aPnt11 = new gp_Pnt(0, 0, 0);
            gp_Pnt aPnt12 = new gp_Pnt(rectangleW, 0, 0);
            gp_Pnt aPnt13 = new gp_Pnt(rectangleW, rectangleL, 0);
            gp_Pnt aPnt14 = new gp_Pnt(0, rectangleL, 0);

            // BASE PART INT
            gp_Pnt aPnt15 = new gp_Pnt(0 + wallThick, 0 + wallThick, 0);
            gp_Pnt aPnt16 = new gp_Pnt(rectangleW - wallThick, 0 + wallThick, 0);
            gp_Pnt aPnt17 = new gp_Pnt(rectangleW - wallThick, rectangleL - wallThick, 0);
            gp_Pnt aPnt18 = new gp_Pnt(0 + wallThick, rectangleL - wallThick, 0);



            // base part ext
            gp_Pnt aPnt21 = new gp_Pnt(0, 0, pipeL);
            gp_Pnt aPnt22 = new gp_Pnt(rectangleW, 0, pipeL);
            gp_Pnt aPnt23 = new gp_Pnt(rectangleW, rectangleL, pipeL);
            gp_Pnt aPnt24 = new gp_Pnt(0, rectangleL, pipeL);

            // BASE PART INT
            gp_Pnt aPnt25 = new gp_Pnt(0 + wallThick, 0 + wallThick, pipeL);
            gp_Pnt aPnt26 = new gp_Pnt(rectangleW - wallThick, 0 + wallThick, pipeL);
            gp_Pnt aPnt27 = new gp_Pnt(rectangleW - wallThick, rectangleL - wallThick, pipeL);
            gp_Pnt aPnt28 = new gp_Pnt(0 + wallThick, rectangleL - wallThick, pipeL);

            List<List<gp_Pnt>> faces = new List<List<gp_Pnt>> { new List<gp_Pnt> { aPnt11, aPnt12, aPnt13, aPnt14 }, new List<gp_Pnt> { aPnt15, aPnt16, aPnt17, aPnt18 }, new List<gp_Pnt> { aPnt21, aPnt22, aPnt23, aPnt24 }, new List<gp_Pnt> { aPnt25, aPnt26, aPnt27, aPnt28 } };

            // sadly u must know how to orientate faces, the algorithm can't determine it by itself for now
            List<TopAbs_Orientation> orientations = new List<TopAbs_Orientation> { TopAbs_Orientation.TopAbs_REVERSED, TopAbs_Orientation.TopAbs_FORWARD };


            List<Face> tempFaces = new List<Face>();



            // top face
            List<gp_Pnt> allPoints = faces[0];
            allPoints.AddRange(faces[1]);
            Surface surface = new Surface(allPoints, orientations[1]);
            tempFaces.AddRange(surface.ComputeFaces());


            // bot face
            allPoints = faces[2];
            allPoints.AddRange(faces[3]);
            surface = new Surface(allPoints, orientations[0]);
            tempFaces.AddRange(surface.ComputeFaces());


            // lateral face exterior
            allPoints = faces[0];
            allPoints.AddRange(faces[2]);
            surface = new Surface(allPoints, orientations[1]);
            tempFaces.AddRange(surface.ComputeFaces());

            // lateral face interior
            allPoints = faces[1];
            allPoints.AddRange(faces[3]);
            surface = new Surface(allPoints, orientations[0]);
            tempFaces.AddRange(surface.ComputeFaces());

            // ordonned pts + triangles
            int count = 0;
            foreach (Face f in tempFaces)
            {
                AddFaces(f.ToTrianglesGraham(true).subFaces);
                count++;
            }

            RecalculateMesh();
        }
    }
}
