Catmull-Clark Mesh Subdivision
-----------------------------------------------------------------

The script implements the Catmull–Clark algorithm of mesh subdivision.
Just use the "Torec/Subdivision" utility in Editor
or the following procedure on development or at runtime:
public Mesh Subdivide(Mesh mesh, int iterations)

Related links:
    https://en.wikipedia.org/wiki/Catmull–Clark_subdivision_surface

-----------------------------------------------------------------

Asset contents:
    Subdivision
        readme.txt                  - This readme.
        Assets
            Subdivision.cs          - The main module implementing the Subdivide() procedure.
        Demo
            SubdivisionDemo.cs      - A demo component building an array of meshes 
                                      demonstrating subdivision of a cube (with some faces missing) 
                                      and different boundary interpolation modes.
            SubdivisionDemo.unity   - SubdivisionDemo component work result.
            Ball                    - Low Polygon Soccer Ball by Ahmet Gencoglu: https://assetstore.unity.com/packages/3d/low-polygon-soccer-ball-84382
        Editor
            SubdivisionUtility.cs   - Subdivision utility window class.

-----------------------------------------------------------------

February 2018

Viktor Massalogin
massalogin@gmail.com
