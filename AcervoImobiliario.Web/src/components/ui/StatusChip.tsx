import { Chip, type ChipProps } from '@mui/material';

interface StatusChipProps extends Omit<ChipProps, 'label' | 'color'> {
  active: boolean;
  label?: string;
}

/** Chip de status padronizado — Ativo usa amarelo da identidade visual. */
export function StatusChip({ active, label, size = 'small', ...props }: StatusChipProps) {
  return (
    <Chip
      size={size}
      label={label ?? (active ? 'Ativo' : 'Inativo')}
      color={active ? 'highlight' : 'default'}
      variant={active ? 'filled' : 'outlined'}
      {...props}
    />
  );
}
