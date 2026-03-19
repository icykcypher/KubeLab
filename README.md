# KubeLab
## Setup
### Setup python for Ansible
```bash
source prepare_ansible_env.sh
```

### Setup Ansible on target machine
```bash
cd ansible/
ansible-playbook -i inventories/prod/hosts.ini setup_ansible.yml
```

### Prepare ansible vault
```bash
ansible-vault create secrets.yml
```

### Deploy cluster
```bash
ansible-playbook -i inventories/prod/hosts.ini  site.yml --ask-vault-password
```
