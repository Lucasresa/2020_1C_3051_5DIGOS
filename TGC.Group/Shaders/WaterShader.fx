
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

float time = 0;
//Input del Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float2 MeshPosition : TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
    VS_OUTPUT Output;
    Output.MeshPosition = Input.Position;
    Input.Position.y += 20 * sin(time + Input.Position.x) + 20 * cos(time + Input.Position.z);
    Output.Position = mul(Input.Position, matWorldViewProj);
    Output.Texcoord = Input.Texcoord;
    return Output;
}

//Pixel Shader
float4 ps_main(VS_OUTPUT input) : COLOR0
{
    float textureScale = 60;
    float2 waterDirection = float2(0.03, 0.03) * time;
    float4 textureColor = tex2D(diffuseMap, input.Texcoord * textureScale + waterDirection);

    textureColor.a = 0.8;
    return textureColor;
}

technique Olas
{
    pass Pass_0
    {
        AlphaBlendEnable = TRUE;
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}