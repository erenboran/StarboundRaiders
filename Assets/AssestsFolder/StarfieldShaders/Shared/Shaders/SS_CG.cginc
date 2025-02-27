inline float4 SkyboxClipPosODS(float3 inPos)
{
    float4 clipPos;
    float3 posWorld = inPos + _WorldSpaceCameraPos.xyz;
#if defined(STEREO_CUBEMAP_RENDER_ON)
    float3 offset = ODSOffset(posWorld, unity_HalfStereoSeparation.x);
    clipPos = mul(UNITY_MATRIX_VP, float4(posWorld + offset, 1.0));
#else
    clipPos = mul(UNITY_MATRIX_VP, float4(posWorld, 1.0));
#endif
    return clipPos;
}

// Tranforms position from object to homogenous space
inline float4 SkyboxClipPos(in float3 pos)
{
#if defined(STEREO_CUBEMAP_RENDER_ON)
    return SkyboxClipPosODS(pos);
#else
    // More efficient than computing M*VP matrix product
    return mul(UNITY_MATRIX_VP, float4(pos + _WorldSpaceCameraPos, 1.0));
#endif
}
inline float4 SkyboxClipPos(float4 pos) // overload for float4; avoids "implicit truncation" warning for existing shaders
{
    return SkyboxClipPos(pos.xyz);
}