
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

/**************************************************************************************/
                                        /* Mar */
/**************************************************************************************/

//Vertex Shader
VS_OUTPUT vs_main_water(VS_INPUT Input)
{
    VS_OUTPUT Output;
    Output.MeshPosition = Input.Position;
    Input.Position.y += 40 * sin(1/20 * (time + Input.Position.x)) + 35 * cos(3 * (time + Input.Position.z));
    Output.Position = mul(Input.Position, matWorldViewProj);
    Output.Texcoord = Input.Texcoord;
    Output.WorldNormal = mul(float4(Input.Normal, 1.0), matInverseTransposeWorld);
    return Output;
}

//Pixel Shader
float4 ps_main_water(VS_OUTPUT input) : COLOR0
{
    float textureScale = 10;
    float2 waterDirection = float2(0.003, 0.003) * time;
    float4 textureColor = tex2D(diffuseMap, input.Texcoord * textureScale + waterDirection);

    textureColor.a = 0.7;
    return textureColor;
}

technique Waves
{
    pass Pass_0
    {
        AlphaBlendEnable = TRUE;
        VertexShader = compile vs_3_0 vs_main_water();
        PixelShader = compile ps_3_0 ps_main_water();
    }
}

/**************************************************************************************/
                                        /* Niebla */
/**************************************************************************************/

texture texFogMap;
sampler2D fogMap = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

// variable de fogs
float4 ColorFog;
float4 CameraPos;
float StartFogDistance;
float EndFogDistance;
float Density;

//Output del Vertex Shader
struct VS_OUTPUT_VERTEX
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float4 PosView : COLOR0;
    float4 TexturePosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
};

VS_OUTPUT_VERTEX vs_main_fog(VS_INPUT input)
{
    VS_OUTPUT_VERTEX output;
    output.Position = mul(input.Position, matWorldViewProj);
    output.Texture = input.Texcoord;
    output.PosView = mul(input.Position, matWorldView);
    output.TexturePosition = mul(input.Position, matWorld);
    output.WorldNormal = mul(float4(input.Normal, 1.0), matInverseTransposeWorld);
    return output;
}

//Pixel Shader
float4 ps_main_fog(VS_OUTPUT_VERTEX input) : COLOR0
{
    float zn = StartFogDistance;
    float zf = EndFogDistance;

    float4 fvBaseColor = tex2D(fogMap, input.Texture);
    
    if (input.TexturePosition.y > 3505)
        return fvBaseColor;
    else
    {
        if (input.PosView.z < zn)
            return fvBaseColor;
        else if (input.PosView.z > zf)
            return ColorFog;
        else
        {
            float total = zf - zn;
            float resto = input.PosView.z - zn;
            float proporcion = resto / total;
            return fvBaseColor * (1 - proporcion)+ ColorFog * proporcion;
        }
    }
}

technique Fog
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main_fog();
        PixelShader = compile ps_3_0 ps_main_fog();
    }
}

/**************************************************************************************/
                                    /* Terreno */
/**************************************************************************************/

texture texReflex;
sampler2D reflex = sampler_state
{
    Texture = (texReflex);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

//Pixel Shader
float4 ps_main_terrain(VS_OUTPUT_VERTEX Input) : COLOR0
{
    float3 Nn = normalize(Input.WorldNormal);
    float3 Ln = normalize(float3(0, -2, 1));
    float n_dot_l = abs(dot(Nn, Ln));
    float textureScale = 200;
    float4 textureColor = tex2D(diffuseMap, Input.Texture * textureScale);
    float3 diffuseColor = 0.4 * float3(0.5, 0.4, 0.2) * n_dot_l;
    textureColor += float4(diffuseColor, 1);
    
    float movement = 0.002 * sin(time * 2);
    float4 reflexTexture = tex2D(reflex, (Input.Texture + float2(1, 1) * movement) * 50);
    
    float4 fvBaseColor = textureColor + reflexTexture * 0.4;
    
    float zn = StartFogDistance;
    float zf = EndFogDistance;
        
    if (Input.PosView.z < zn)
        return fvBaseColor;
    else if (Input.PosView.z > zf)
        return ColorFog;
    else
    {
        float total = zf - zn;
        float resto = Input.PosView.z - zn;
        float proporcion = resto / total;
        return fvBaseColor * (1 - proporcion) + ColorFog * proporcion;
    }
}

technique DiffuseMap
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main_fog();
        PixelShader = compile ps_3_0 ps_main_terrain();
    }
}