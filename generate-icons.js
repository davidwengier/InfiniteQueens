const { createCanvas } = require('canvas');
const fs = require('fs');

function drawLogo(size) {
    const canvas = createCanvas(size, size);
    const ctx = canvas.getContext('2d');
    const scale = size / 512;
    
    ctx.save();
    ctx.scale(scale, scale);
    
    // Background - gradient (approximated with solid color for simplicity)
    ctx.fillStyle = '#2196F3';
    ctx.fillRect(0, 0, 512, 512);
    
    // Draw infinity symbol
    ctx.strokeStyle = 'rgba(255, 255, 255, 0.2)';
    ctx.lineWidth = 40;
    ctx.lineCap = 'round';
    
    // Infinity path
    ctx.beginPath();
    const cx = 256;
    const cy = 256;
    const radius = 120;
    
    for (let t = 0; t <= Math.PI * 2; t += 0.01) {
        const x = cx + radius * Math.cos(t) / (1 + Math.sin(t) * Math.sin(t));
        const y = cy + radius * Math.sin(t) * Math.cos(t) / (1 + Math.sin(t) * Math.sin(t));
        if (t === 0) {
            ctx.moveTo(x, y);
        } else {
            ctx.lineTo(x, y);
        }
    }
    ctx.stroke();
    
    // Draw chess queen
    ctx.fillStyle = 'white';
    const queenCenterX = 256;
    const queenCenterY = 240;
    const queenSize = 140;
    
    // Crown top
    ctx.beginPath();
    ctx.moveTo(queenCenterX, queenCenterY - queenSize / 2);
    ctx.lineTo(queenCenterX - queenSize / 4, queenCenterY - queenSize / 3);
    ctx.lineTo(queenCenterX - queenSize / 6, queenCenterY - queenSize / 4);
    ctx.lineTo(queenCenterX, queenCenterY - queenSize / 5);
    ctx.lineTo(queenCenterX + queenSize / 6, queenCenterY - queenSize / 4);
    ctx.lineTo(queenCenterX + queenSize / 4, queenCenterY - queenSize / 3);
    ctx.lineTo(queenCenterX, queenCenterY - queenSize / 2);
    ctx.closePath();
    ctx.fill();
    
    // Crown body
    ctx.beginPath();
    ctx.moveTo(queenCenterX - queenSize / 3, queenCenterY - queenSize / 5);
    ctx.lineTo(queenCenterX - queenSize / 4, queenCenterY + queenSize / 6);
    ctx.lineTo(queenCenterX + queenSize / 4, queenCenterY + queenSize / 6);
    ctx.lineTo(queenCenterX + queenSize / 3, queenCenterY - queenSize / 5);
    ctx.closePath();
    ctx.fill();
    
    // Base
    ctx.beginPath();
    ctx.moveTo(queenCenterX - queenSize / 2.2, queenCenterY + queenSize / 6);
    ctx.lineTo(queenCenterX - queenSize / 2.5, queenCenterY + queenSize / 3);
    ctx.lineTo(queenCenterX + queenSize / 2.5, queenCenterY + queenSize / 3);
    ctx.lineTo(queenCenterX + queenSize / 2.2, queenCenterY + queenSize / 6);
    ctx.closePath();
    ctx.fill();
    
    // Add sparkle at top
    ctx.fillStyle = '#FFD700';
    ctx.beginPath();
    ctx.arc(queenCenterX, queenCenterY - queenSize / 2, 8, 0, Math.PI * 2);
    ctx.fill();
    
    ctx.restore();
    
    return canvas;
}

// Generate icons
const sizes = [
    { size: 512, name: 'icon-512.png' },
    { size: 192, name: 'icon-192.png' },
    { size: 180, name: 'apple-touch-icon.png' },
    { size: 32, name: 'favicon-32x32.png' },
    { size: 16, name: 'favicon.png' }
];

sizes.forEach(({ size, name }) => {
    const canvas = drawLogo(size);
    const buffer = canvas.toBuffer('image/png');
    fs.writeFileSync(`InfiniteQueens.Web/wwwroot/${name}`, buffer);
    console.log(`Generated ${name}`);
});

console.log('All icons generated!');
