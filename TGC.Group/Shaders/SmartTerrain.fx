
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
    
    float movement = 0.002 * sin(time * 2);
    float4 reflexTexture = tex2D(reflex, (Input.Texcoord + float2(1, 1) * movement) * 50);
    
    return textureColor + reflexTexture * 0.4;
}

technique DiffuseMap
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main_terrain();
        PixelShader = compile ps_3_0 ps_main_terrain();
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
};

VS_OUTPUT_VERTEX vs_main_fog(VS_INPUT input)
{
    VS_OUTPUT_VERTEX output;
    output.Position = mul(input.Position, matWorldViewProj);
    output.Texture = input.Texcoord;
    output.PosView = mul(input.Position, matWorldView);
    output.TexturePosition = mul(input.Position, matWorld);
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
        {
            fvBaseColor = ColorFog;
            return fvBaseColor;
        }
        else
        {
	    	// combino fog y textura
            float1 total = zf - zn;
            float1 resto = input.PosView.z - zn;
            float1 proporcion = resto / total;
            fvBaseColor = lerp(fvBaseColor, ColorFog, proporcion);
            return fvBaseColor;
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
