use serde_json::{Map, Value};
use std::{
	collections::HashSet,
	env,
	error::Error,
	io::{Error as IoError, ErrorKind},
};
use windbot_rs::{load_cards, run};
use ygopro_cdb_reader::{Card, path};

#[tokio::main]
async fn main() -> Result<(), Box<dyn Error>> {
	let args: Vec<String> = env::args().skip(1).collect();
	let mut database_path: &str = "cards.cdb";
	let mut seen: HashSet<String> = HashSet::new();
	let mut options: Map<String, Value> = Map::new();
	for argument in &args {
		let (key, value): (&str, &str) = argument.split_once('=').ok_or_else(|| {
			IoError::new(ErrorKind::InvalidInput, format!("参数缺少 '='：{argument}"))
		})?;
		let key: String = key.trim().to_ascii_uppercase();
		let value: &str = value.trim();
		if !seen.insert(key.clone()) {
			return Err(IoError::new(
				ErrorKind::InvalidInput,
				format!("重复参数：{key}"),
			).into());
		}
		if key == "CARDDATAPATH" {
			if value.is_empty() {
				return Err(IoError::new(
					ErrorKind::InvalidInput,
					"CardDataPath 不能为空",
				).into());
			}
			database_path = value;
			continue;
		}
		let field: &str = match key.as_str() {
			"NAME" => "Name",
			"DECK" => "Deck",
			"DECKFILE" => "DeckFile",
			"DIALOG" => "Dialog",
			"HOST" => "Host",
			"HOSTINFO" => "HostInfo",
			"PORT" => "Port",
			"VERSION" => "Version",
			"HAND" => "Hand",
			"DEBUG" => "Debug",
			"CHAT" => "Chat",
			"CONFIG" | "SERVERMODE" | "SERVERPORT" => {
				return Err(IoError::new(
					ErrorKind::InvalidInput,
					format!("当前单机器人入口不支持参数：{key}"),
				).into());
			}
			_ => continue,
		};
		let parsed: Value = match field {
			"Port" | "Version" | "Hand" => {
				let number: i32 = if let Some(hex) = value.strip_prefix("0x") {
					u32::from_str_radix(hex, 16)? as i32
				} else {
					value.parse::<i32>()?
				};
				Value::from(number)
			}
			"Debug" | "Chat" => {
				let enabled: bool = if value.eq_ignore_ascii_case("true") {
					true
				} else if value.eq_ignore_ascii_case("false") {
					false
				} else {
					return Err(IoError::new(
						ErrorKind::InvalidInput,
						format!("{field} 必须为 True 或 False"),
					).into());
				};
				Value::Bool(enabled)
			}
			_ => Value::String(value.to_owned()),
		};
		options.insert(field.to_owned(), parsed);
	}
	let json: String = serde_json::to_string(&options)?;
	let cards: Vec<Card> = path(database_path).await?;
	load_cards(&cards)?;
	run(Some(&json))?;
	Ok(())
}
