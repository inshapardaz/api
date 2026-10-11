#!/usr/bin/env python3
"""One-off move of LanguageTools' data into the API.

  * Transliteration dictionary (LanguageTools' Data/dictionary.json) -> the API's per-language
    transliteration entries: each entry's Urdu spelling goes to "ur", its Hindi spelling to "hi".
  * Meaning ("natural") dictionaries -> /dictionaries, owned by the account the script logs in as.
    Either re-upload the original StarDict/DSL files (--source-dir, preferred: LanguageTools' DSL
    importer attached every earlier headword of a file to each entry as an alternate, so its stored
    copies of DSL dictionaries are wrong), or export each dictionary from a running LanguageTools
    (--languagetools-url) and upload the export.

LanguageTools' spell-check replacement history is not moved: it has no language, and the API
rebuilds it as writers pick suggestions.

Standard library only. Example:

  API_EMAIL=admin@example.com API_PASSWORD=... python3 migrate.py \\
      --api-url https://api.nawishta.co.uk \\
      --transliteration-json /home/data/languagetools/dictionary.json \\
      --source-dir ./original-dictionaries   # or: --languagetools-url http://localhost:5000

Run with --dry-run first to see what would be sent.
"""
import argparse
import json
import mimetypes
import os
import sys
import urllib.error
import urllib.request
import uuid
from pathlib import Path

DICTIONARY_SUFFIXES = ('.zip', '.tar.gz', '.tgz', '.dsl', '.dsl.dz')


def request(method, url, token=None, body=None, content_type='application/json', timeout=900):
    headers = {'Accept': 'application/json'}
    if token:
        headers['Authorization'] = f'Bearer {token}'
    if body is not None:
        headers['Content-Type'] = content_type
        if content_type == 'application/json':
            body = json.dumps(body).encode('utf-8')
    req = urllib.request.Request(url, data=body, method=method, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=timeout) as response:
            return response.read(), response.headers
    except urllib.error.HTTPError as error:
        detail = error.read().decode('utf-8', 'replace')[:500]
        raise SystemExit(f'{method} {url} failed with {error.code}: {detail}')


def login(api_url):
    email, password = os.environ.get('API_EMAIL'), os.environ.get('API_PASSWORD')
    if not email or not password:
        raise SystemExit('Set API_EMAIL and API_PASSWORD for an Admin (or DictionaryWriter) account.')
    content, _ = request('POST', f'{api_url}/accounts/authenticate', body={'email': email, 'password': password})
    return json.loads(content)['accessToken']


def multipart(fields, file_field, file_name, file_bytes):
    boundary = uuid.uuid4().hex
    parts = []
    for name, value in fields.items():
        if value:
            parts.append(f'--{boundary}\r\nContent-Disposition: form-data; name="{name}"\r\n\r\n{value}\r\n'.encode('utf-8'))
    file_type = mimetypes.guess_type(file_name)[0] or 'application/octet-stream'
    parts.append(f'--{boundary}\r\nContent-Disposition: form-data; name="{file_field}"; filename="{file_name}"\r\n'
                 f'Content-Type: {file_type}\r\n\r\n'.encode('utf-8') + file_bytes + b'\r\n')
    parts.append(f'--{boundary}--\r\n'.encode('utf-8'))
    return b''.join(parts), f'multipart/form-data; boundary={boundary}'


def migrate_transliteration(api_url, token, path, dry_run):
    entries = json.loads(Path(path).read_text(encoding='utf-8-sig'))
    for language, field in (('ur', 'urdu'), ('hi', 'hindi')):
        batch = [{'roman': e['roman'], 'native': e[field], 'meaning': e.get('meaning'), 'category': e.get('category')}
                 for e in entries if e.get('roman') and e.get(field)]
        print(f'transliteration/{language}: {len(batch)} entries')
        if dry_run or not batch:
            continue
        content, _ = request('POST', f'{api_url}/tools/{language}/transliteration/entries/bulk', token, batch)
        print(f'  -> {json.loads(content)}')


def upload_dictionary(api_url, token, file_name, file_bytes, fields, dry_run):
    print(f'dictionary: {fields.get("name") or file_name} ({len(file_bytes):,} bytes)')
    if dry_run:
        return
    body, content_type = multipart(fields, 'file', file_name, file_bytes)
    content, _ = request('POST', f'{api_url}/dictionaries/upload', token, body, content_type)
    created = json.loads(content)
    print(f'  -> id {created["id"]}, {created["entryCount"]} entries')


def migrate_source_files(api_url, token, source_dir, dry_run):
    files = sorted(p for p in Path(source_dir).iterdir() if p.is_file() and p.name.lower().endswith(DICTIONARY_SUFFIXES))
    if not files:
        print(f'no dictionary files ({", ".join(DICTIONARY_SUFFIXES)}) in {source_dir}')
    for path in files:
        upload_dictionary(api_url, token, path.name, path.read_bytes(), {}, dry_run)


def migrate_from_languagetools(api_url, token, languagetools_url, dry_run):
    content, _ = request('GET', f'{languagetools_url}/api/natural-dictionary')
    listing = json.loads(content)
    dictionaries = listing.get('dictionaries', listing) if isinstance(listing, dict) else listing
    for dictionary in dictionaries:
        exported, _ = request('GET', f'{languagetools_url}/api/natural-dictionary/{dictionary["id"]}/export?format=dsl')
        fields = {
            'name': dictionary.get('name'),
            'sourceLanguage': dictionary.get('sourceLanguage'),
            'targetLanguage': dictionary.get('targetLanguage'),
        }
        upload_dictionary(api_url, token, f'{dictionary["id"]}.zip', exported, fields, dry_run)


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--api-url', required=True)
    parser.add_argument('--transliteration-json', help="LanguageTools' Data/dictionary.json")
    source = parser.add_mutually_exclusive_group()
    source.add_argument('--source-dir', help='folder of original StarDict/DSL archives to upload')
    source.add_argument('--languagetools-url', help='running LanguageTools to export dictionaries from')
    parser.add_argument('--dry-run', action='store_true', help='read and report, send nothing to the API')
    args = parser.parse_args()

    api_url = args.api_url.rstrip('/')
    token = None if args.dry_run else login(api_url)

    if args.transliteration_json:
        migrate_transliteration(api_url, token, args.transliteration_json, args.dry_run)
    if args.source_dir:
        migrate_source_files(api_url, token, args.source_dir, args.dry_run)
    if args.languagetools_url:
        migrate_from_languagetools(api_url, token, args.languagetools_url.rstrip('/'), args.dry_run)


if __name__ == '__main__':
    sys.exit(main())
