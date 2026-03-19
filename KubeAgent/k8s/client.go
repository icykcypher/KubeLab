package k8s

import (
	"context"
	"log"

	appsv1 "k8s.io/api/apps/v1"
	v1 "k8s.io/api/core/v1"
	metav1 "k8s.io/apimachinery/pkg/apis/meta/v1"

	"k8s.io/client-go/kubernetes"
	"k8s.io/client-go/rest"
)

type Client struct {
	clientset *kubernetes.Clientset
}

func NewClient() *Client {
	config, err := rest.InClusterConfig()
	if err != nil {
		log.Fatal(err)
	}

	clientset, err := kubernetes.NewForConfig(config)
	if err != nil {
		log.Fatal(err)
	}

	return &Client{clientset: clientset}
}

func (c *Client) CreateDeployment(namespace string, name string, image string) {
	deploymentsClient := c.clientset.AppsV1().Deployments(namespace)

	deployment := &appsv1.Deployment{
		ObjectMeta: metav1.ObjectMeta{
			Name: name,
		},
		Spec: appsv1.DeploymentSpec{
			Replicas: int32Ptr(1),
			Selector: &metav1.LabelSelector{
				MatchLabels: map[string]string{"app": name},
			},
			Template: v1.PodTemplateSpec{
				ObjectMeta: metav1.ObjectMeta{
					Labels: map[string]string{"app": name},
				},
				Spec: v1.PodSpec{
					Containers: []v1.Container{
						{
							Name:  name,
							Image: image,
							Ports: []v1.ContainerPort{
								{ContainerPort: 80},
							},
						},
					},
				},
			},
		},
	}

	_, err := deploymentsClient.Create(context.TODO(), deployment, metav1.CreateOptions{})
	if err != nil {
		log.Println("Failed to create deployment:", err)
	}
}

func int32Ptr(i int32) *int32 { return &i }
