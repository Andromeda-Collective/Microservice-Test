from django.contrib import admin
from.models import ProductModel

# Register your models here.
@admin.register(ProductModel)
class ProductAdmin(admin.ModelAdmin):
    list_display = ["name", "status", "created_date", "updated_date"]
    search_fields = ["name"]

