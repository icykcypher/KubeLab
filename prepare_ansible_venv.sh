#!/usr/bin/env bash
# prepare_ansible_env.sh
# Universal Python environment setup for Ansible with Docker and Kubernetes support

set -e

# Directory for virtual environment
VENV_DIR=".ansible_env"

# Function to detect package manager and install system dependencies
install_dependencies() {
    echo "Detecting OS and installing system dependencies..."

    if [ -f /etc/debian_version ]; then
        # Debian / Ubuntu
        sudo apt update
        sudo apt install -y python3 python3-venv python3-pip python3-dev build-essential libffi-dev
    elif [ -f /etc/arch-release ]; then
        # Arch Linux
        sudo pacman -Sy --noconfirm python python-virtualenv python-pip base-devel libffi
    elif [ -f /etc/fedora-release ]; then
        # Fedora
        sudo dnf install -y python3 python3-venv python3-pip python3-devel gcc libffi-devel redhat-rpm-config
    else
        echo "Unsupported OS. Please install Python 3, venv, and pip manually."
        exit 1
    fi
}

# Ensure python3 is installed
if ! command -v python3 >/dev/null 2>&1; then
    install_dependencies
fi

# Create virtual environment if it does not exist
if [ ! -d "$VENV_DIR" ]; then
    python3 -m venv "$VENV_DIR"
    echo "Virtual environment created at $VENV_DIR"
fi

# Activate virtual environment in current shell
# Important: run this script with `source prepare_ansible_env.sh`
source "$VENV_DIR/bin/activate"

# Upgrade pip and wheel
pip install --upgrade pip wheel setuptools

# Install Ansible with Docker and Kubernetes support
pip install "ansible[docker,k8s,openshift]" --upgrade

# Additional dependencies for Docker/K8s modules
pip install docker kubernetes openshift

echo "Ansible environment is ready and activated."
echo "Virtual environment directory: $VENV_DIR"
