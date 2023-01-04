set VAULT_ADDR=https://vault.aue1e.saasure.net
vault login --no-print=true --method=aws role=releng-build-machines-container-role


vault kv get -field=value releng/services/internal/code_sign/sn/BASE64_releng_sn_keypair.snk > BASE64_releng_sn_keypair.snk
certutil -decode BASE64_releng_sn_keypair.snk releng_sn_keypair.snk

"C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v10.0A\\bin\\NETFX 4.8.1 Tools\\sn.exe" -d  OktaDotnetStrongname

"C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v10.0A\\bin\\NETFX 4.8.1 Tools\\sn.exe" -i releng_sn_keypair.snk OktaDotnetStrongname