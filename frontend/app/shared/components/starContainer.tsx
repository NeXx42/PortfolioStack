"use client"

import { useEffect, useRef } from "react";

export default function () {
    const canvas = useRef<HTMLCanvasElement>(null);

    useEffect(() => {
        if (canvas.current === null)
            return;

        const ctx = canvas.current.getContext('2d')!;
        let W: number;
        let H: number;
        let particles: any[] = [];
        let raf: number;

        const GLYPHS = ['✦', '✧', '⋆', '∗', '◈', '⬡', '⟡', '✴'];
        const COLS = [
            [126, 200, 227], // sky blue
            [168, 221, 240], // sky2
            [91, 184, 216],  // sky3
            [200, 230, 245], // pale blue-white
            [180, 160, 240], // faint violet accent
        ];

        function rnd(a: number, b: number) { return a + Math.random() * (b - a); }
        function pick(arr: any[]) { return arr[Math.floor(Math.random() * arr.length)]; }

        function mkParticle() {
            const type = Math.random() < 0.5 ? 'star'
                : Math.random() < 0.5 ? 'twinkle'
                    : Math.random() < 0.6 ? 'mote'
                        : 'glyph';
            const col = pick(COLS);
            return {
                type,
                x: rnd(0, W),
                y: rnd(0, H),
                // individual speeds — very slow drift
                vx: rnd(-0.08, 0.08),
                vy: rnd(-0.12, -0.02),
                col,
                // star/mote
                r: type === 'mote' ? rnd(0.4, 1.2)
                    : type === 'glyph' ? rnd(7, 13)
                        : rnd(0.6, 2.2),
                // base opacity
                a: rnd(0.04, 0.22),
                // twinkle phase (each particle is out of phase)
                phase: rnd(0, Math.PI * 2),
                // twinkle speed
                speed: rnd(0.3, 1.1),
                // for glyph type
                glyph: pick(GLYPHS),
                rot: rnd(0, Math.PI * 2),
                rotV: rnd(-0.005, 0.005),
                // scale pulse for twinkle type
                scale: 1,
                life: rnd(0, 1),
                lifeV: rnd(0.002, 0.006),
            };
        }

        W = canvas.current!.width = 1920;
        H = canvas.current!.height = 1080;
        particles = [];

        for (let i = 0; i < 250; i++)
            particles.push(mkParticle());

        function tick(t: number) {
            ctx.clearRect(0, 0, W, H);

            for (const p of particles) {
                // drift
                p.x += p.vx;
                p.y += p.vy;
                p.phase += 0.01 * p.speed;
                p.life += p.lifeV;

                // wrap
                if (p.x < -20) p.x = W + 20;
                if (p.x > W + 20) p.x = -20;
                if (p.y < -20) { p.x = rnd(0, W); p.y = H + 20; }

                // animated opacity — slow sine pulse
                const pulse = 0.5 + 0.5 * Math.sin(p.phase);

                if (p.type === 'star') {
                    // soft glow disc + optional cross sparkle
                    const a = p.a * pulse;
                    const grd = ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.r * 4);
                    grd.addColorStop(0, `rgba(${p.col[0]},${p.col[1]},${p.col[2]},${a * 0.9})`);
                    grd.addColorStop(0.4, `rgba(${p.col[0]},${p.col[1]},${p.col[2]},${a * 0.3})`);
                    grd.addColorStop(1, `rgba(${p.col[0]},${p.col[1]},${p.col[2]},0)`);
                    ctx.fillStyle = grd;
                    ctx.beginPath();
                    ctx.arc(p.x, p.y, p.r * 4, 0, Math.PI * 2);
                    ctx.fill();
                    // bright centre
                    ctx.fillStyle = `rgba(${p.col[0]},${p.col[1]},${p.col[2]},${Math.min(a * 2, 0.5)})`;
                    ctx.beginPath();
                    ctx.arc(p.x, p.y, p.r * 0.5, 0, Math.PI * 2);
                    ctx.fill();

                } else if (p.type === 'twinkle') {
                    // sharp 4-point starburst that pulses in size
                    const sz = p.r * (0.5 + pulse * 1.5);
                    const a = p.a * pulse * 1.4;
                    // outer soft halo first
                    const grd = ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, sz * 5);
                    grd.addColorStop(0, `rgba(${p.col[0]},${p.col[1]},${p.col[2]},${a * 0.25})`);
                    grd.addColorStop(1, `rgba(${p.col[0]},${p.col[1]},${p.col[2]},0)`);
                    ctx.fillStyle = grd;
                    ctx.beginPath();
                    ctx.arc(p.x, p.y, sz * 5, 0, Math.PI * 2);
                    ctx.fill();
                    // cross beams
                    ctx.save();
                    ctx.translate(p.x, p.y);
                    ctx.rotate(Math.PI / 4 * pulse * 0.2); // very slight rotation on pulse
                    ctx.strokeStyle = `rgba(${p.col[0]},${p.col[1]},${p.col[2]},${a})`;
                    ctx.lineWidth = sz * 0.25;
                    ctx.lineCap = 'round';
                    for (let b = 0; b < 2; b++) {
                        ctx.save();
                        ctx.rotate(b * Math.PI / 2);
                        ctx.beginPath();
                        ctx.moveTo(0, -sz * 2.2);
                        ctx.lineTo(0, sz * 2.2);
                        ctx.stroke();
                        ctx.restore();
                    }
                    // centre dot
                    ctx.fillStyle = `rgba(255,255,255,${a * 0.8})`;
                    ctx.beginPath();
                    ctx.arc(0, 0, sz * 0.35, 0, Math.PI * 2);
                    ctx.fill();
                    ctx.restore();

                } else if (p.type === 'mote') {
                    // tiny fast drift particle — just a soft circle
                    const a = p.a * pulse;
                    ctx.fillStyle = `rgba(${p.col[0]},${p.col[1]},${p.col[2]},${a})`;
                    ctx.beginPath();
                    ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
                    ctx.fill();

                } else if (p.type === 'glyph') {
                    // rotating rune/symbol
                    p.rot += p.rotV;
                    const a = p.a * pulse * 0.85;
                    ctx.save();
                    ctx.translate(p.x, p.y);
                    ctx.rotate(p.rot);
                    ctx.font = `${p.r}px serif`;
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';
                    ctx.fillStyle = `rgba(${p.col[0]},${p.col[1]},${p.col[2]},${a})`;
                    ctx.shadowColor = `rgba(${p.col[0]},${p.col[1]},${p.col[2]},${a * 0.5})`;
                    ctx.shadowBlur = 6;
                    ctx.fillText(p.glyph, 0, 0);
                    ctx.restore();
                }
            }

            raf = requestAnimationFrame(tick);
        }

        window.addEventListener('load', () => { tick(0); });
    }, [])

    return (<canvas ref={canvas} id="StarContainer" />)
}