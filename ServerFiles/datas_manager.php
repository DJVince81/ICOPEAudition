<?php
$xmlData = file_get_contents('php://input');

if (!file_exists("data.xml")) {
    $file = fopen("data.xml", "w");
    fclose($file);
}

$file = fopen("data.xml", "w");
fwrite($file, $xmlData);
fclose($file);

$xml = simplexml_load_file('data.xml');

if (!file_exists("data.csv")) {
    $csv = fopen('data.csv', 'w');
    fwrite($csv, "\xEF\xBB\xBF");
    fclose($csv);
}

$csv = fopen('data.csv', 'w');
fwrite($csv, "\xEF\xBB\xBF");
foreach ($xml->Item as $item) {
    $type = (string)$item['Type'];
    $value = trim((string)$item);
    if ($type === "System.DateTime") {
        $value = date('Y-m-d H:i:s', strtotime($value));
    }
    fputcsv($csv, array($type, $value), ';');
}
fclose($csv);

echo "Saving xml and csv files completed !";
?>