fn main() -> Result<(), windbot_rs::WindBotError> {
	println!("{}", windbot_rs::list_bots()?);
	Ok(())
}
