
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
    float3 Normal : NORMAL0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 MeshPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
};

//Vertex Shader
VS_OUTPUT vs_main_water(VS_INPUT Input)
{
    VS_OUTPUT Output;
    Output.MeshPosition = Input.Position;
    Input.Position.y += 30 * sin(time + Input.Position.x) + 25 * cos( 5 * time + Input.Position.z);
    Output.Position = mul(Input.Position, matWorldViewProj);
    Output.Texcoord = Input.Texcoord;
    Output.WorldNormal = mul(float4(Input.Normal, 1.0), matInverseTransposeWorld);
    return Output;
}

//Pixel Shader
float4 ps_main_water(VS_OUTPUT input) : COLOR0
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
        VertexShader = compile vs_3_0 vs_main_water();
        PixelShader = compile ps_3_0 ps_main_water();
    }
}

/**************************************************************************************/
/* Terreno */
/**************************************************************************************/

//Vertex Shader
VS_OUTPUT vs_main_terrain(VS_INPUT Input)
{
    VS_OUTPUT Output;
    Output.Position = mul(Input.Position, matWorldViewProj);
    Output.Texcoord = Input.Texcoord;
    Output.MeshPosition = Input.Position;
    Output.WorldNormal = mul(float4(Input.Normal, 1.0), matInverseTransposeWorld);
    return Output;
}

//Pixel Shader
float4 ps_main_terrain(VS_OUTPUT Input) : COLOR0
{
    float3 Nn = normalize(Input.WorldNormal);
    float3 Ln = normalize(float3(0, -2, 1));

    float n_dot_l = abs(dot(Nn, Ln));

    float textureScale = 200;
    float4 textureColor = tex2D(diffuseMap, Input.Texcoord * textureScale);
	
    float3 diffuseColor = 0.4 * float3(0.5, 0.4, 0.2) * n_dot_l;
    textureColor += float4(diffuseColor, 1);
    
    return textureColor;
}

technique DiffuseMap
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main_terrain();
        PixelShader = compile ps_3_0 ps_main_terrain();
    }
}