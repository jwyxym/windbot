//! Rust interface to the precompiled WindBot NativeAOT library.
//! build.rs selects a CI static artifact using Cargo's target triple.

use std::{
	error::Error,
	ffi::{CStr, c_char, c_void},
	fmt,
};
pub use ygopro_cdb_reader::Card;

mod ffi {
	use super::{c_char, c_void};

	unsafe extern "C" {
		pub fn windbot_load_cards(json: *const u8, length: usize) -> i32;
		pub fn windbot_run(json: *const u8, length: usize) -> i32;
		pub fn windbot_list() -> *mut c_char;
		pub fn windbot_last_error() -> *mut c_char;
		pub fn windbot_free(value: *mut c_void);
	}
}

#[derive(Debug)]
pub struct WindBotError(String);

impl fmt::Display for WindBotError {
	fn fmt(&self, formatter: &mut fmt::Formatter<'_>) -> fmt::Result {
		formatter.write_str(&self.0)
	}
}

impl Error for WindBotError {}

/// Serializes the supplied cards to JSON and replaces the global card database.
/// Call before starting bots. Replacement fails while any bot is running.
pub fn load_cards_json(cards: &Vec<Card>) -> Result<(), WindBotError> {
	let json: Vec<u8> = serde_json::to_vec(cards)
		.map_err(|error: serde_json::Error| WindBotError(error.to_string()))?;
	let status: i32 = unsafe { ffi::windbot_load_cards(json.as_ptr(), json.len()) };
	check_status(status)
}

/// Returns JSON rows [Chinese name, AI name, deck, dialog, level].
pub fn list_bots_json() -> Result<String, WindBotError> {
	let pointer: *mut c_char = unsafe { ffi::windbot_list() };
	if pointer.is_null() {
		return Err(last_error());
	}
	take_string(pointer)
}

/// Blocks until the bot disconnects. Run on a worker thread.
/// None uses {}: localhost:7911, a random Normal AI and its embedded resources.
/// Card data must already have been loaded with load_cards_json.
pub fn run(options: Option<&str>) -> Result<(), WindBotError> {
	let json: &str = options.unwrap_or("{}");
	let status: i32 = unsafe { ffi::windbot_run(json.as_ptr(), json.len()) };
	check_status(status)
}

fn check_status(status: i32) -> Result<(), WindBotError> {
	if status == 0 {
		Ok(())
	} else {
		Err(last_error())
	}
}

fn last_error() -> WindBotError {
	let pointer: *mut c_char = unsafe { ffi::windbot_last_error() };
	if pointer.is_null() {
		return WindBotError("WindBot returned no error message.".into());
	}
	match take_string(pointer) {
		Ok(message) => WindBotError(message),
		Err(error) => error,
	}
}

// Only accepts pointers returned by WindBot's allocated-string exports.
fn take_string(pointer: *mut c_char) -> Result<String, WindBotError> {
	let result: Result<String, WindBotError> = unsafe { CStr::from_ptr(pointer) }
		.to_str()
		.map(str::to_owned)
		.map_err(|error: std::str::Utf8Error| WindBotError(error.to_string()));
	unsafe { ffi::windbot_free(pointer.cast()) };
	result
}
