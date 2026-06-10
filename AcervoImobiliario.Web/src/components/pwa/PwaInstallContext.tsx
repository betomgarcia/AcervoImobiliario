import { createContext, useContext, useMemo, useState, type ReactNode } from 'react';

interface PwaInstallContextValue {
  bottomOffset: number;
  setBottomOffset: (offset: number) => void;
}

const PwaInstallContext = createContext<PwaInstallContextValue | null>(null);

export function PwaInstallProvider({ children }: { children: ReactNode }) {
  const [bottomOffset, setBottomOffset] = useState(0);

  const value = useMemo(
    () => ({ bottomOffset, setBottomOffset }),
    [bottomOffset],
  );

  return (
    <PwaInstallContext.Provider value={value}>{children}</PwaInstallContext.Provider>
  );
}

export function usePwaInstallBottomOffset(): number {
  const context = useContext(PwaInstallContext);
  return context?.bottomOffset ?? 0;
}

export function usePwaInstallLayout(): PwaInstallContextValue {
  const context = useContext(PwaInstallContext);
  if (!context) {
    throw new Error('usePwaInstallLayout deve ser usado dentro de PwaInstallProvider.');
  }
  return context;
}
