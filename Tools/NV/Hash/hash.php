<?php
// Small PHP Reference for CellAO Cryptography to create and verify hashs

// createhash("passwordhere") to create a hash for the password
// verifyhash("hashhere", "passwordhere") to verify that the password is correct


$hash = "1234$857152b9f35e3cde45d375c3d6f73e01";
$password = "asdf";

if (verifyhash($hash, $password))
{
	echo "valid password!\n";
} else
{
	echo "invalid password!\n";
}

function verifyhash($hash, $password)
{
	$parts = explode("$", $hash);

	$newhash = createhash($password, $parts[0]);

	if ($newhash == $hash)
		return true;

	return false;
}

function createhash($password, $salt = "")
{
	if ($salt == "")
		$salt = sprintf('%02x%02x', mt_rand(0, 0xff), mt_rand(0, 0xff));

	return strtolower($salt) . "$" . md5(pack("H*", $salt) . $password);
}
?>
