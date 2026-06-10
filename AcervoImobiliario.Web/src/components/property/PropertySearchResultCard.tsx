import HistoryEduIcon from '@mui/icons-material/HistoryEdu';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import VisibilityIcon from '@mui/icons-material/Visibility';
import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  Divider,
  Stack,
  Typography,
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { StatusChip } from '@/components/ui/StatusChip';
import type { PropertyResponse } from '@/types/api';
import { formatFullPropertyAddress } from '@/utils/format';
import { tokens } from '@/theme/tokens';

interface PropertySearchResultCardProps {
  property: PropertyResponse;
}

export function PropertySearchResultCard({ property }: PropertySearchResultCardProps) {
  const fullAddress = formatFullPropertyAddress(property);
  const complementText = property.complement?.trim();

  return (
    <Card
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        '&:hover': { boxShadow: tokens.shadow.cardHover },
      }}
    >
      <CardContent sx={{ flex: 1, p: 2.5, pb: 2 }}>
        <Stack direction="row" spacing={1.5} alignItems="flex-start" sx={{ mb: 2 }}>
          <Box
            sx={{
              width: 36,
              height: 36,
              borderRadius: `${tokens.radius.sm}px`,
              bgcolor: tokens.color.primary,
              color: tokens.color.textOnPrimary,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              flexShrink: 0,
            }}
          >
            <LocationOnIcon fontSize="small" />
          </Box>
          <Box sx={{ flex: 1, minWidth: 0 }}>
            <Typography variant="subtitle1" sx={{ lineHeight: 1.45 }}>
              {fullAddress}
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
              {property.neighborhood} · {property.cityNameSnapshot}
            </Typography>
            {property.cadastralIndex ? (
              <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                Índice cadastral: {property.cadastralIndex}
              </Typography>
            ) : null}
            {complementText ? (
              <Typography variant="body2" color="text.secondary" sx={{ mt: 0.25 }}>
                {complementText}
              </Typography>
            ) : null}
          </Box>
        </Stack>
        <StatusChip active={property.isActive} />
      </CardContent>

      <Divider />

      <CardActions
        sx={{
          p: 2,
          pt: 1.5,
          gap: 1,
          bgcolor: tokens.color.background,
          flexDirection: { xs: 'column', sm: 'row' },
          '& > :not(:first-of-type)': { ml: { xs: 0, sm: 0 } },
        }}
      >
        <Button
          component={RouterLink}
          to={`/imoveis/${property.id}`}
          variant="outlined"
          color="primary"
          size="medium"
          startIcon={<VisibilityIcon />}
          fullWidth
        >
          Detalhes
        </Button>
        <Button
          component={RouterLink}
          to={`/imoveis/${property.id}/historico`}
          variant="contained"
          color="primary"
          size="medium"
          startIcon={<HistoryEduIcon />}
          fullWidth
        >
          Histórico
        </Button>
      </CardActions>
    </Card>
  );
}
