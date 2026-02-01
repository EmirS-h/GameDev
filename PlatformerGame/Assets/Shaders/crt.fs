#version 330

in vec2 fragTexCoord;
out vec4 finalColor;

uniform sampler2D texture0;
uniform float u_time;
uniform vec2 u_texture_size;
uniform float u_aberration_strength; 
uniform float u_dither_strength;     
uniform float u_color_depth;
uniform float u_noise_strength; // 0.0 = Off, 0.05 = Subtle Static

const float CURVATURE = 6.0;
const float MASK_STRENGTH = 0.2;

// Pseudo-random function for static noise
float rand(vec2 co) {
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec2 curve(vec2 uv) {
    uv = (uv - 0.5) * 2.0;
    uv *= 1.05; 
    uv.x *= 1.0 + pow((abs(uv.y) / CURVATURE), 2.0);
    uv.y *= 1.0 + pow((abs(uv.x) / CURVATURE), 2.0);
    return (uv / 2.0) + 0.5;
}

void main() {
    vec2 uv = curve(fragTexCoord);
    
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) {
        finalColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }

    // 1. Chromatic Aberration
    float dist = distance(uv, vec2(0.5));
    vec2 offset = vec2(dist * 0.003 * u_aberration_strength, 0.0);
    vec3 color;
    color.r = texture(texture0, uv + offset).r;
    color.g = texture(texture0, uv).g;
    color.b = texture(texture0, uv - offset).b;

    // 2. Moving Scanlines
    // Added u_time * 10.0 to make them crawl
    float scanline = sin((uv.y * u_texture_size.y * 1.5) - (u_time * 10.0));
    color -= scanline * 0.05;

    // 3. Static Noise (Toggleable)
    if (u_noise_strength > 0.0) {
        float noise = rand(uv * u_time); // Seed with time for movement
        color += (noise - 0.5) * u_noise_strength;
    }

    // 4. Quantization & Dither
    if (u_color_depth > 0.0) {
        color = floor(color * u_color_depth) / u_color_depth;
    }

    // 5. Vertical Shadow Mask
    float mask = 1.0 - MASK_STRENGTH;
    if (mod(gl_FragCoord.x, 3.0) < 1.0) color.r *= mask;
    else if (mod(gl_FragCoord.x, 3.0) < 2.0) color.g *= mask;
    else color.b *= mask;

    // 6. Vignette
    vec2 v = uv * (1.0 - uv.yx);
    float vig = v.x * v.y * 15.0;
    color *= pow(vig, 0.15);

    finalColor = vec4(color, 1.0);
}