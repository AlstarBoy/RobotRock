Shader"Custom/StencilMask"
{
    SubShader
    {
        // Render before other geometry
        Tags { "Queue" = "Geometry-1" }

        // Don't draw color, only update stencil
ColorMask0
        ZWriteOn

        Pass
        {
            Stencil
            {
Ref1
                Compalways
                Pass replace
            }
        }
    }
}

