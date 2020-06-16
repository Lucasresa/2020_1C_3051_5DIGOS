
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

float time = 0;
float4 CameraPos;

// variable de fogs
float4 ColorFog;
float StartFogDistance;
float EndFogDistance;

// Parametros de la Luz
float3 diffuseColor; //Color RGB para Ambient de la luz
float3 specularColor; //Color RGB para Ambient de la luz
float4 lightPosition; //Posicion de la luz
float4 eyePosition; //Posicion de la camara
float specularExp; // Shininess

float3 shipAmbientColor; // Color de ambiente para la nave
float shipKSpecular; // Coeficiente de luz especular para la nave

float3 goldAmbientColor; // Color de ambiente para el oro
float goldKSpecular; // Coeficiente de luz especular para el oro

float3 silverAmbientColor; // Color de ambiente para la plata
float silverKSpecular; // Coeficiente de luz especular para la plata

float3 ironAmbientColor; // Color de ambiente para el hierro
float ironKSpecular; // Coeficiente de luz especular para el hierro

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
    float4 MeshPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
};


//Output del Vertex Shader
struct VS_OUTPUT_VERTEX
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float4 PosView : COLOR0;
    float4 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
};


float get_fog_amount(float3 viewDirection, float fogStart, float fogRange)
{
    return saturate((length(viewDirection) - fogStart) / fogRange);
}

/**************************************************************************************/
                                        /* Mar */
/**************************************************************************************/

//Vertex Shader
VS_OUTPUT_VERTEX vs_main_water(VS_INPUT Input)
{
    VS_OUTPUT_VERTEX Output;
    float dx = Input.Position.x;
    float dy = Input.Position.z;
    float freq = sqrt(dx * dx + dy * dy);
    float amp = 30;
    float angle = -time * 3 + freq / 300;
    Input.Position.y += sin(angle) * amp;
    Output.Position = mul(Input.Position, matWorldViewProj);
    Output.Texture = Input.Texcoord;
    Output.PosView = mul(Input.Position, matWorldView);
    Output.WorldPosition = mul(Input.Position, matWorld);
    Output.WorldNormal = mul(Input.Normal, matInverseTransposeWorld).xyz;
    return Output;
}

//Pixel Shader
float4 ps_main_water(VS_OUTPUT_VERTEX input) : COLOR0
{
    float textureScale = 10;
    float2 waterDirection = float2(.003, .003) * time;
    float4 textureColor = tex2D(diffuseMap, input.Texture * textureScale + waterDirection);
    float distance = distance(input.WorldPosition.xz, CameraPos.xz) / 1500;
    textureColor.a = clamp(1 - 1 / distance, .3, .9);
    
    float zn = StartFogDistance;
    float zf = EndFogDistance;
    
    float3 viewDirection = CameraPos.xyz - input.WorldPosition.xyz;
    float FogAmount = get_fog_amount(viewDirection, StartFogDistance, (EndFogDistance - StartFogDistance));
    
    if (input.PosView.z < zn)
        return textureColor;
    else if (input.PosView.z > zf)
        return ColorFog;
    else
        return lerp(textureColor, ColorFog, FogAmount);    
    
}

technique Waves
{
    pass Pass_0
    {
        AlphaBlendEnable = true;
        VertexShader = compile vs_3_0 vs_main_water();
        PixelShader = compile ps_3_0 ps_main_water();
    }
}

/**************************************************************************************/
                                        /* Niebla */
/**************************************************************************************/

float4 calculate_fog(VS_OUTPUT_VERTEX input)
{
    float zn = StartFogDistance;
    float zf = EndFogDistance;

    float4 fvBaseColor = tex2D(fogMap, input.Texture);
    
    if (input.WorldPosition.y > 3550)
        return fvBaseColor;
    else
    {
        float3 viewDirection = CameraPos.xyz - input.WorldPosition.xyz;
        float FogAmount = get_fog_amount(viewDirection, StartFogDistance, (EndFogDistance - StartFogDistance));
        
        if (input.PosView.z < zn)
            return fvBaseColor;
        else if (input.PosView.z > zf)
            return ColorFog;
        else
            return lerp(fvBaseColor, ColorFog, FogAmount);
    }
}

float4 calculate_light(VS_OUTPUT_VERTEX input, float3 ambientColor, float specularK)
{
    /* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
    input.WorldNormal = normalize(input.WorldNormal);
    
    float3 lightDirection = normalize(lightPosition - input.WorldPosition);
    float3 viewDirection = normalize(eyePosition - input.WorldPosition);
    float3 halfVector = normalize(lightDirection + viewDirection);

	// Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texture);

	//Componente Diffuse: N dot L
    float3 NdotL = dot(input.WorldNormal, lightDirection);
    float3 diffuseLight = 0.6 * diffuseColor * max(0.0, NdotL);

	//Componente Specular: (N dot H)^shininess
    float3 NdotH = dot(input.WorldNormal, halfVector);
    float3 specularLight = ((NdotL <= 0.0) ? 0.0 : specularK) * specularColor * pow(max(0.0, NdotH), specularExp);

    float4 finalColor = float4(saturate(ambientColor * 0.3 + diffuseLight) * texelColor + specularLight, texelColor.a);
    return finalColor;
}

VS_OUTPUT_VERTEX vs_main_fog(VS_INPUT input)
{
    VS_OUTPUT_VERTEX output;
    output.Position = mul(input.Position, matWorldViewProj);
    output.Texture = input.Texcoord;
    output.PosView = mul(input.Position, matWorldView);
    output.WorldPosition = mul(input.Position, matWorld);
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    return output;
}

float4 ps_main_fog(VS_OUTPUT_VERTEX input) : COLOR0
{
    return calculate_fog(input);
}

//Pixel Shader
float4 ps_main_fog_vegetation(VS_OUTPUT_VERTEX input) : COLOR0
{
    float zn = StartFogDistance;
    float zf = EndFogDistance;
    
    float4 fvBaseColor = tex2D(fogMap, input.Texture);
    float4 Color;
    
    float3 viewDirection = CameraPos.xyz - input.WorldPosition.xyz;
    float FogAmount = get_fog_amount(viewDirection, StartFogDistance, (EndFogDistance - StartFogDistance));
    
    if (input.PosView.z < zn)
        return fvBaseColor;
    else if (input.PosView.z > zf)
        return ColorFog;
    else
    {
        if (fvBaseColor.a < 0.4)
            discard;
        else
            return lerp(fvBaseColor, ColorFog, FogAmount);
    }
}

float4 ps_main_fog_bubble(VS_OUTPUT_VERTEX input) : COLOR0
{
    float4 fog = calculate_fog(input);
    fog.a = 0.2;
    return fog;
}

technique Fog
{
    pass Pass_0
    {       
        VertexShader = compile vs_3_0 vs_main_fog();
        PixelShader = compile ps_3_0 ps_main_fog();
    }
}

technique FogVegetation
{
    pass Pass_0
    {       
        VertexShader = compile vs_3_0 vs_main_fog();
        PixelShader = compile ps_3_0 ps_main_fog_vegetation();
    }
}

technique FogBubble
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main_fog();
        PixelShader = compile ps_3_0 ps_main_fog_bubble();
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
    
    float movement = 0.001 * sin(time * 2);
    float4 reflexTexture = tex2D(reflex, (Input.Texture + float2(1, 1) * movement) * 50);
    
    float4 fvBaseColor = textureColor * 0.9 + reflexTexture * 0.4;
    
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

/**************************************************************************************/
                                    /* Sun */
/**************************************************************************************/

//Pixel Shader
float4 ps_sun(VS_OUTPUT_VERTEX Input) : COLOR0
{
    float4 texel = tex2D(diffuseMap, Input.Texture);
    float4 color = distance(Input.Texture.xy, .4);
		
    color = 1 / color;
    if (distance(Input.Texture.xy, .5) > 0.8)
        return texel;
    else
        return lerp(texel, color, 0.0070);
}

technique Sun
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main_fog();
        PixelShader = compile ps_3_0 ps_sun();
    }
}

/**************************************************************************************/
                                    /* Ship */
/**************************************************************************************/

//Pixel Shader
float4 ps_main_ship(VS_OUTPUT_VERTEX input) : COLOR0
{
    return calculate_light(input, shipAmbientColor, shipKSpecular);
}

technique Ship_Light
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main_fog();
        PixelShader = compile ps_3_0 ps_main_ship();
    }
}
