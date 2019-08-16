using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TopAbs;

namespace OCCTest.Elements
{
    public class PipeRectangular: BasicElement
    {


        /// <summary>
        /// generate a tube with a cubic shape
        /// </summary>
        /// <param name="myWidth">width</param>
        /// <param name="myHeight">height</param>
        /// <param name="myThickness">thickness</param>
        /// <param name="myLength">length</param>
        //public CubicTube(double myWidth, double myHeight, double myThickness, double myLength)
        public PipeRectangular(double wallThickness, double pipeLength, double rectangleLength, double rectangleWidth)
        {// note that u could have achieved the same by creating a wire, then make it slide on a spline to create a pipe, then cut external and internal shapes and finally triangulate it (example in the elbow file)

            // inferior part
            // base part ext
            gp_Pnt aPnt11 = new gp_Pnt(0, 0, 0);
            gp_Pnt aPnt12 = new gp_Pnt(rectangleWidth, 0, 0);
            gp_Pnt aPnt13 = new gp_Pnt(rectangleWidth, rectangleLength, 0);
            gp_Pnt aPnt14 = new gp_Pnt(0, rectangleLength, 0);

            // BASE PART INT
            gp_Pnt aPnt15 = new gp_Pnt(  0 + wallThickness, 		0 + wallThickness, 		0);
            gp_Pnt aPnt16 = new gp_Pnt(rectangleWidth - wallThickness, 	0 + wallThickness, 		0);
            gp_Pnt aPnt17 = new gp_Pnt(rectangleWidth - wallThickness, rectangleLength - wallThickness, 	0);
            gp_Pnt aPnt18 = new gp_Pnt(  0 + wallThickness, rectangleLength - wallThickness, 	0);



            // base part ext
            gp_Pnt aPnt21 = new gp_Pnt(0, 0, pipeLength);
            gp_Pnt aPnt22 = new gp_Pnt(rectangleWidth, 0, pipeLength);
            gp_Pnt aPnt23 = new gp_Pnt(rectangleWidth, rectangleLength, pipeLength);
            gp_Pnt aPnt24 = new gp_Pnt(0, rectangleLength, pipeLength);

            // BASE PART INT
            gp_Pnt aPnt25 = new gp_Pnt(0 + wallThickness, 0 + wallThickness, pipeLength);
            gp_Pnt aPnt26 = new gp_Pnt(rectangleWidth - wallThickness, 0 + wallThickness, pipeLength);
            gp_Pnt aPnt27 = new gp_Pnt(rectangleWidth - wallThickness, rectangleLength - wallThickness, pipeLength);
            gp_Pnt aPnt28 = new gp_Pnt(0 + wallThickness, rectangleLength - wallThickness, pipeLength);

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
                myFaces.AddRange(f.ToTrianglesGraham(true).subFaces);
                count++;
            }

        }
    }
}
