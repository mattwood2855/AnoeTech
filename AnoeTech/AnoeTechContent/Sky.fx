float4x4 xWorldMatrix;
float4x4 xViewMatrix;
float4x4 xProjectionMatrix;

float4 AmbientColor;
float4 AmbientIntensity;

struct VertexToPixel
{
	float4       Position : POSITION;
	float4          Color : COLOR0;
	float  LightIntensity : TEXCOORD0;
	float2  TextureCoords : TEXCOORD1;
};

struct PixelToFrame
{
	float4 Color : COLOR0;
};

VertexToPixel VertexShaderFunction(float4 inPos : POSITION, float2 inTexCoords: TEXCOORD0)
{
    VertexToPixel output = (VertexToPixel)0;

    float4 worldPosition = mul(inPos, xWorldMatrix);
    float4 viewPosition  = mul(worldPosition, xViewMatrix);
    output.Position  = mul(viewPosition, xProjectionMatrix);

    return output;
}

PixelToFrame PixelShaderFunction(VertexToPixel input)
{
	PixelToFrame output = (PixelToFrame)0;

    return output;
}

technique Sky
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}


