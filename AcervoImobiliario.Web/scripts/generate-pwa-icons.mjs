import { readFileSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import sharp from 'sharp';

const rootDir = path.dirname(path.dirname(fileURLToPath(import.meta.url)));
const publicDir = path.join(rootDir, 'public');
const source = readFileSync(path.join(publicDir, 'icon-source.svg'));

const outputs = [
  { file: 'apple-touch-icon.png', size: 180 },
  { file: 'pwa-192x192.png', size: 192 },
  { file: 'pwa-512x512.png', size: 512 },
  { file: 'favicon-32x32.png', size: 32 },
];

for (const { file, size } of outputs) {
  await sharp(source).resize(size, size).png().toFile(path.join(publicDir, file));
  console.log(`Generated ${file}`);
}

await sharp(source).resize(64, 64).png().toFile(path.join(publicDir, 'favicon.png'));
console.log('Generated favicon.png');
