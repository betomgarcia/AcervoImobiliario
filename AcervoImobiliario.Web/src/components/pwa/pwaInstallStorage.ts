const MINIMIZED_KEY = 'acervo-pwa-install-minimized';

export function readPwaInstallMinimized(): boolean {
  try {
    return localStorage.getItem(MINIMIZED_KEY) === 'true';
  } catch {
    return false;
  }
}

export function writePwaInstallMinimized(minimized: boolean): void {
  try {
    if (minimized) {
      localStorage.setItem(MINIMIZED_KEY, 'true');
    } else {
      localStorage.removeItem(MINIMIZED_KEY);
    }
  } catch {
    // localStorage indisponível — ignora
  }
}
