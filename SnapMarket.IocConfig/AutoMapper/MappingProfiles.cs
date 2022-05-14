﻿using AutoMapper;
using SnapMarket.Entities;
using SnapMarket.ViewModels;
using SnapMarket.ViewModels.Cart;
using SnapMarket.ViewModels.Order;
using SnapMarket.Entities.Identity;
using SnapMarket.ViewModels.Manage;
using SnapMarket.ViewModels.Slider;
using SnapMarket.ViewModels.Product;
using SnapMarket.ViewModels.Category;
using SnapMarket.ViewModels.Comments;
using SnapMarket.ViewModels.Newsletter;
using SnapMarket.ViewModels.RoleManager;
using SnapMarket.ViewModels.UserManager;

namespace SnapMarket.IocConfig.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UsersViewModel>().ReverseMap()
                .ForMember(m => m.Claims, opt => opt.Ignore());

            CreateMap<Role, RolesViewModel>().ReverseMap()
                .ForMember(m => m.UserRoles, opt => opt.Ignore())
                .ForMember(m => m.Claims, opt => opt.Ignore());

            CreateMap<Product, ProductViewModel>().ReverseMap()
                .ForMember(m => m.SellerId, opt => opt.Ignore());

            CreateMap<Category, CategoryViewModel>().ReverseMap()
                .ForMember(m => m.Parent, opt => opt.Ignore())
                .ForMember(m => m.SubCategories, opt => opt.Ignore())
                .ForMember(m => m.ProductCategories, opt => opt.Ignore());

            CreateMap<Seller, SellerViewModel>().ReverseMap()
                .ForMember(m => m.Products, opt => opt.Ignore());

            CreateMap<Comment, CommentViewModel>().ReverseMap()
                .ForMember(m => m.Product, opt => opt.Ignore());

            CreateMap<Cart, CartViewModel>().ReverseMap()
                .ForMember(m => m.IsComplete, opt => opt.Ignore())
                .ForMember(m => m.BrowserId, opt => opt.Ignore());

            CreateMap<User, EmailViewModel>().ReverseMap()
                .ForMember(m => m.Claims, opt => opt.Ignore());

            CreateMap<User, ProfileViewModel>().ReverseMap()
                .ForMember(m => m.Claims, opt => opt.Ignore());

            CreateMap<MessageUser, MessageUsersViewModel>().ReverseMap();

            CreateMap<CreditCart, CreditCartViewModel>().ReverseMap();

            CreateMap<Newsletter, NewsletterViewModel>().ReverseMap();

            CreateMap<Discount, DiscountViewModel>().ReverseMap();

            CreateMap<Order, OrderViewModel>().ReverseMap();

            CreateMap<Slider, SliderViewModel>().ReverseMap();
        }
    }
}
