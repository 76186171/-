using System;
using System.Collections.Generic;
using System.Text;
using BlogDemo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogDemo.Infrastructure.Database.EntityConfigurations
{
   public  class PostConfiguration: IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(x => x.Remark).HasMaxLength(200);
        }
    }
}
