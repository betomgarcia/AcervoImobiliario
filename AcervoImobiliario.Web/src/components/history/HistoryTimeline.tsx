import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Box,
  Chip,
  Divider,
  Stack,
  Typography,
} from '@mui/material';
import type { PropertyHistoryResponse } from '@/types/api';
import { formatDate, formatDateTime, summarizeText } from '@/utils/format';
import { historyEventTypeColors, historyEventTypeLabels } from '@/utils/labels';

interface HistoryTimelineProps {
  histories: PropertyHistoryResponse[];
}

export function HistoryTimeline({ histories }: HistoryTimelineProps) {
  if (histories.length === 0) {
    return null;
  }

  return (
    <Box sx={{ position: 'relative', pl: { xs: 0.5, sm: 1 } }}>
      <Box
        aria-hidden
        sx={{
          position: 'absolute',
          top: 20,
          bottom: 20,
          left: { xs: 15, sm: 23 },
          width: 4,
          borderRadius: 999,
          background: 'linear-gradient(180deg, #1565C0 0%, #00838F 100%)',
          opacity: 0.25,
        }}
      />

      <Stack spacing={2.5}>
        {histories.map((history) => {
          const color = historyEventTypeColors[history.eventType];
          const eventLabel = historyEventTypeLabels[history.eventType];
          const summary = summarizeText(history.description);

          return (
            <Box key={history.id} sx={{ position: 'relative', pl: { xs: 4.5, sm: 6 } }}>
              <Box
                aria-hidden
                sx={{
                  position: 'absolute',
                  left: { xs: 6, sm: 14 },
                  top: 24,
                  width: 16,
                  height: 16,
                  borderRadius: '50%',
                  bgcolor: color,
                  border: '3px solid',
                  borderColor: 'background.paper',
                  boxShadow: `0 0 0 4px ${color}22`,
                  zIndex: 1,
                }}
              />

              <Accordion
                disableGutters
                elevation={0}
                sx={{
                  borderRadius: '16px !important',
                  border: '1px solid',
                  borderColor: 'divider',
                  bgcolor: 'background.paper',
                  transition: 'box-shadow 0.2s ease, border-color 0.2s ease',
                  overflow: 'hidden',
                  '&:before': { display: 'none' },
                  '&.Mui-expanded': {
                    borderColor: `${color}55`,
                    boxShadow: `0 8px 24px ${color}14`,
                  },
                }}
              >
                <AccordionSummary
                  expandIcon={<ExpandMoreIcon />}
                  sx={{
                    px: { xs: 2, sm: 2.5 },
                    py: 1,
                    '& .MuiAccordionSummary-content': { my: 1 },
                  }}
                >
                  <Stack spacing={1} sx={{ width: '100%', pr: 1 }}>
                    <Stack
                      direction={{ xs: 'column', sm: 'row' }}
                      spacing={1}
                      alignItems={{ xs: 'flex-start', sm: 'center' }}
                      flexWrap="wrap"
                      useFlexGap
                    >
                      <Chip
                        label={eventLabel}
                        size="small"
                        sx={{
                          bgcolor: `${color}18`,
                          color,
                          fontWeight: 700,
                        }}
                      />
                      <Typography variant="subtitle2" fontWeight={700} color="text.primary">
                        {formatDate(history.eventDate)}
                      </Typography>
                    </Stack>

                    <Typography
                      variant="body2"
                      color="text.secondary"
                      sx={{
                        display: '-webkit-box',
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: 'vertical',
                        overflow: 'hidden',
                        lineHeight: 1.5,
                      }}
                    >
                      {summary}
                    </Typography>
                  </Stack>
                </AccordionSummary>

                <AccordionDetails sx={{ px: { xs: 2, sm: 2.5 }, pt: 0, pb: 2.5 }}>
                  <Divider sx={{ mb: 2 }} />
                  <Stack spacing={1.5}>
                    <Box>
                      <Typography variant="caption" color="text.secondary" display="block" gutterBottom>
                        Descrição completa
                      </Typography>
                      <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap', lineHeight: 1.6 }}>
                        {history.description}
                      </Typography>
                    </Box>
                    <Typography variant="caption" color="text.secondary">
                      Registrado em {formatDateTime(history.createdAt)}
                    </Typography>
                  </Stack>
                </AccordionDetails>
              </Accordion>
            </Box>
          );
        })}
      </Stack>
    </Box>
  );
}
