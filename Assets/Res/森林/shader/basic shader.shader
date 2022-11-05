Shader "Adventure Forest/basic"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [Toggle] _Wind("Wind", Float) = 1
        _WindPower ("Wind Power", Range(0,1)) = 0.5
        
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Cull off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard addshadow vertex:vert
        

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        float _Wind;
        float _WindPower;
        

        struct Input
        {
            float2 uv_MainTex;
            
        };

       

        
        fixed4 _Color;

         void vert (inout appdata_full v) {
            if(_Wind > 0){
				float3 vertexWorld = mul (unity_ObjectToWorld, v.vertex);

                float height = v.vertex.z / 0.017;
                v.vertex.y += sin(_Time.g * 3 + vertexWorld.z) * height * 0.01 * _WindPower;
			}
                
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            clip(c.a - 0.1);
            // Metallic and smoothness come from slider variables
            o.Alpha = c.a;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
