using System.Collections.Generic;
using TopAbs;

namespace OpencascadePart
{
    /// <summary>
    /// a mesh containing unique vertices and triangles 
    /// to use it in the client: call Client.MyMesh (so not this version of MyMesh) with triangles and vertices as parameters (get it from BasicElement.MyMesh.triangles/vertices)
    /// </summary>
    public class MyMesh
    {
        public List<Pnt> vertices; // array for vertices // set in retrievesIndex()
        public int[] triangles; // triangles

        private List<TopAbs_Orientation> orientations = new List<TopAbs_Orientation>(); // faces' orientations
        private List<List<int>> tri; // temporary array for triangles

        /// <summary>
        /// empty constructor
        /// </summary>
        public MyMesh()
        {
            vertices = new List<Pnt>();
            triangles = new int[0];
        }

        /// <summary>
        /// create vertices and triangles with the given faces
        /// </summary>
        /// <param name="objFaces">the faces of our mesh</param>
        public MyMesh(List<Face> objFaces)
        {

            // set tri and orientations (and also vertices indirectly by calling retrievesIndex()
            ReArrangeVertices(objFaces);

            // can you set the orientations before with ReArrangeVertices(List<Face>) please :) 
            // get the triangles (takes the orientation in consideration) and add them to the mesh
            triangles = ComputeTrianglesAndOrientations(tri);

            // output = { int[] triangles and List<Pnt> vertices }
        }
        
        /// <summary>
        /// compute the triangles by reversing the faces(tri=triangles) or not (depending on their orientations)
        /// </summary>
        /// <param name="tri"></param>
        public int[] ComputeTrianglesAndOrientations(List<List<int>> tri)
        {
            
            int count = 0;
            int[] triangles = new int[tri.Count * 3];
            int faceCount = 0;
            foreach (List<int> set in tri)
            {
                List<int> mySet; 
                if (orientations[faceCount] == TopAbs_Orientation.TopAbs_FORWARD)
                    mySet = set;
                else
                    mySet = ReverseTriangle(set);
                // reverse the set or not depending on the orientation
                
                // then compute the triangles
                foreach (int pt in mySet)
                {
                    triangles[count] = pt;
                    count++;
                }
                faceCount++;
            }

            return triangles;
        }

        /// <summary>
        /// reverse the triangle ( 0 1 2 -> 0 2 1 )
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<int> ReverseTriangle(List<int> v)
        {
            return new List<int> { v[0], v[2], v[1] };
        }


        /// <summary>
        /// tri will contain all the unique indexes of our points for each face, while orientations will get all the orientations
        /// </summary>
        /// <param name="objFaces"></param>
        private void ReArrangeVertices(List<Face> objFaces)
        {

            // clear/initialize
            vertices = new List<Pnt>(); // each unique vertex
            tri = new List<List<int>>(); // vertex indices


            foreach (Face face in objFaces) // Graham already applied at that point
            {
                List<int> tempTri = new List<int>();
                foreach (Pnt p in face.pts)
                {
                    tempTri.Add(RetrievesIndex(p));
                }
                tri.Add(tempTri);
                orientations.Add(face.orientation);

            }
        }


        /// <summary>
        ///  retrieves the index of the vertex (pt)
        /// </summary>
        /// <param name="pt"> the vertex for which we want to know the index</param>
        /// <returns>returns the index of pt</returns>
        public int RetrievesIndex(Pnt pt)
        {
            // if we don't know that vertex, we add it to the vertices
            if (ExistsIn(vertices, pt) == -1)
            {
                vertices.Add(pt);
                return vertices.Count - 1;
            }
            else
            {
                // otherwise we get the index of that already known vertex
                return ExistsIn(vertices, pt);
            }
        }//*/


        /// <summary>
        /// we want to know if the vertex "what" already exists in our list of unique vertices "from"
        /// </summary>
        /// <param name="from"> the list of unique vertices</param>
        /// <param name="what"> the vertex we are looking at</param>
        /// <returns>returns the index of the vertex or -1 if it is the first time we see that vertex</returns>
        public int ExistsIn(List<Pnt> from, Pnt what)
        {
            for (int i = 0; i < from.Count; i++)
            {
                if (from[i].x == what.x && from[i].y == what.y && from[i].z == what.z)
                {
                    return i;
                }
            }
            return -1;
        }//*/


    }
}
