fn main() -> Result<(), windbot::WindBotError> {
	println!("{}", windbot::list_bots_json()?);
	Ok(())
}
