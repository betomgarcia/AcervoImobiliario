import { Box, type BoxProps } from '@mui/material';
import { tokens } from '@/theme/tokens';

interface PageContainerProps extends BoxProps {
  children: React.ReactNode;
}

/** Container principal com largura máxima e espaçamento consistente. */
export function PageContainer({ children, sx, ...props }: PageContainerProps) {
  return (
    <Box
      sx={{
        width: '100%',
        maxWidth: tokens.layout.maxContentWidth,
        mx: 'auto',
        ...sx,
      }}
      {...props}
    >
      {children}
    </Box>
  );
}
