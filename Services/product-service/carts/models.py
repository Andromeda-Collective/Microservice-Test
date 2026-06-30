from django.db import models

# Create your models here.




class ProductModel(models.Model):

    class ProductStatusType(models.TextChoices):
        publish = "show", "نمایش"
        hidden = "hidden", "در انتظار نمایش"
        rejected = "rejected", "رد شده"

    slug = models.SlugField(unique=True)
    name = models.CharField(max_length=255)
    description = models.TextField()
    image = models.ImageField(upload_to="media/product")

    status = models.CharField(choices=ProductStatusType.choices, default=ProductStatusType.hidden.value)
    created_date = models.DateTimeField(auto_now_add=True)
    updated_date = models.DateTimeField()

    def __str__(self):
        return self.name
    
    class Meta:
        ordering = ["-created_date"]

