#ifndef WINDBOT_H
#define WINDBOT_H
#include <stddef.h>
#include <stdint.h>

#ifdef _WIN32
#define WINDBOT_CALL __cdecl
#else
#define WINDBOT_CALL
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* UTF-8 JSON array from serde_json::to_vec(&Vec<ygopro_cdb_reader::Card>).
 * Copies all data before returning; caller retains ownership of json.
 * Replaces the entire card database atomically; forbidden while bots run.
 * Duplicate codes: last record wins. Return 0 on success, 1 on failure. */
int32_t WINDBOT_CALL windbot_load_cards(const uint8_t *json, size_t length);

/* UTF-8 JSON WindBotInfo options, e.g. {"Deck":"Blue-Eyes","Port":7911}.
 * Blocks until the bot disconnects; use a host worker thread for each bot.
 * Call windbot_load_cards first. Return 0 on success, 1 on failure. */
int32_t WINDBOT_CALL windbot_run(const uint8_t *json, size_t length);

/* Returns a newly allocated UTF-8 error for this calling thread.
 * Release with windbot_free. */
char *WINDBOT_CALL windbot_last_error(void);
/* Allocated UTF-8 JSON array: [Chinese bot name, AI name, deck name, dialog name, level]. */
char *WINDBOT_CALL windbot_list(void);
void WINDBOT_CALL windbot_free(void *value);

#ifdef __cplusplus
}
#endif
#endif
