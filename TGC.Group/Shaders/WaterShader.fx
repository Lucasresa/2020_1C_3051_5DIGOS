/*
* Shader con tecnicas varias utilizadas por diversas herramientas del framework,
* como: TgcBox, TgcArrow, TgcPlaneWall, TgcBoundingBox, TgcBoundingSphere, etc.
* Hay varias Techniques, una para cada combinacion utilizada en el framework de formato de vertice:
*	- PositionColoredTextured
*	- PositionTextured
*	- PositionColored
*	- PositionColoredAlpha
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
    ADDRESSU = MIRROR;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};


//Input del Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 Normal : NORMAL0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 MeshPosition : TEXCOORD1;
    float3 WorldPosition : TEXCOORD2;
    float3 WorldNormal : TEXCOORD3;
};

//Vertex Shader
VS_OUTPUT vsDefault(VS_INPUT input)
{
    VS_OUTPUT output;

    output.Position = mul(input.Position, matWorldViewProj);

    output.Texcoord = input.Texcoord;
    output.MeshPosition = input.Position;
    output.WorldPosition = mul(input.Position, matWorld);
    output.WorldNormal = mul(float4(input.Normal, 1.0), matInverseTransposeWorld);

    return output;
}

//Pixel Shader
float4 psDefault(VS_OUTPUT input) : COLOR0
{
    float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(float3(0, -2, 1));

    float n_dot_l = abs(dot(Nn, Ln));

    float textureScale = 90;
    float4 textureColor = tex2D(diffuseMap, input.Texcoord * textureScale);
	
	// Diffuse color
    float3 diffuseColor = 0.4 * float3(0.5, 0.4, 0.2) * n_dot_l;
    textureColor += float4(diffuseColor, 0.5);
	
    return textureColor;
}

technique Default
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vsDefault();
        PixelShader = compile ps_3_0 psDefault();
    }
}