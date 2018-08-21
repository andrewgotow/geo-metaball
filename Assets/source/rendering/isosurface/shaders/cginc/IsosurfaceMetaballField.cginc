#ifndef METABALL_ISOSURFACE_INCLUDED
#define METABALL_ISOSURFACE_INCLUDED

#define MAX_CHARGES 128

int _Metaball_NumCharges;
float4 _Metaball_Charges[MAX_CHARGES]; // array of float4( position.xyz, radius.w );

float4 metaball (float3 wPos, float3 wCenter, float radius) {
    float3 d = wCenter - wPos;
    float distSqr = dot(d,d);

    float4 o;
    o.w = (radius * radius) / distSqr;
    o.xyz = normalize(d) * o.w;
    return o;
}

float4 sampleField (float3 wPos) {
    float4 field = 0;
    for (int i = 0; i < min(_Metaball_NumCharges,MAX_CHARGES); i ++) {
        field += metaball( wPos, _Metaball_Charges[i].xyz, _Metaball_Charges[i].w );
    }
    field.xyz = normalize(field.xyz);
    return field;
}

#endif