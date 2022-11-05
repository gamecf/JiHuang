Shader "Adventure Forest/water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Color (RGB) Alpha (A)", 2D) = "white"{}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _FlowSpeed ("Flow Speed", Range(0,0.5)) = 0.5
        _WaveStrength ("Wave Strength", Range(0,1)) = 0.5
    
    }
    SubShader
    {
         Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert addshadow alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        float _FlowSpeed;
        float _WaveStrength;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void vert (inout appdata_full v) {
            v.vertex.y += sin(_Time.g * 5 + v.vertex.x * 5 ) * 0.1 * _WaveStrength;
            v.vertex.z += sin(_Time.g * 5 + v.vertex.x * 5 ) * 0.1 * _WaveStrength;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            IN.uv_MainTex.y += _Time.g * _FlowSpeed;
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            clip(c.a - 0.1);
            o.Albedo = c.rgb;
            o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}