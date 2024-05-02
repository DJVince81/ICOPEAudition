<?php
function encodePointComma($chaine) {
    return preg_replace_callback('/"([^"]*)"/', function($match) {
        return '"' . str_replace(';', '_*_POINT_COMMA_*_', $match[1]) . '"';
    }, $chaine);
}

function isValidUUID($uuid) {
    return preg_match('/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i', $uuid);
}

if(isset($_GET['uuid'])) {
    $uuid = $_GET['uuid'];

    if(isValidUUID($uuid)) {
        $file = fopen("data.csv", "r");
        if ($file) {
            $lineFound = false;
            fseek($file, 0);
            while ((($line = fgets($file)) !== false) || ($lineFound !== true)) {
                if (strlen($line) >= 39 && strlen($uuid) >= 36 && str_contains(trim(substr($line, 0, 39)), trim(substr($uuid, 0, 36)))) {
                    echo encodePointComma($line);
                    $lineFound = true;
                }
            }
            fclose($file);
        } else {
            echo "Error: Unable to open CSV file.";
        }
    } else {
        echo "Error: Invalid UUID.";
    }
} else {
    echo "Error: UUID parameter is missing.";
}
?>
