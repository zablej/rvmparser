internal static class DefineConstants
{
	public const int RAPIDJSON_MAJOR_VERSION = 1;
	public const int RAPIDJSON_MINOR_VERSION = 1;
	public const int RAPIDJSON_PATCH_VERSION = 0;
#if ! RAPIDJSON_HAS_STDSTRING && RAPIDJSON_DOXYGEN_RUNNING
	public const int RAPIDJSON_HAS_STDSTRING = 1; // force generation of documentation
#endif
#if ! RAPIDJSON_HAS_STDSTRING && ! RAPIDJSON_DOXYGEN_RUNNING
	public const int RAPIDJSON_HAS_STDSTRING = 0; // no std::string support by default
#endif
	public const int RAPIDJSON_LITTLEENDIAN = 0; //!< Little endian machine
	public const int RAPIDJSON_BIGENDIAN = 1; //!< Big endian machine
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int _ENDIAN_H = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int _FEATURES_H = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ( _BSD_SOURCE || _SVID_SOURCE) && ! _DEFAULT_SOURCE
	public const int _DEFAULT_SOURCE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _ISOC95_SOURCE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _ISOC99_SOURCE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _ISOC11_SOURCE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _POSIX_SOURCE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _POSIX_C_SOURCE = 200809;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ((! __STRICT_ANSI__ || ( _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) >= 500)) && ! _POSIX_SOURCE && ! _POSIX_C_SOURCE) && _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) < 500
	public const int _POSIX_C_SOURCE = 2;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ((! __STRICT_ANSI__ || ( _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) >= 500)) && ! _POSIX_SOURCE && ! _POSIX_C_SOURCE) && ! (_XOPEN_SOURCE && (_XOPEN_SOURCE - 0) < 500) && (_XOPEN_SOURCE && (_XOPEN_SOURCE - 0) < 600)
	public const int _POSIX_C_SOURCE = 199506;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ((! __STRICT_ANSI__ || ( _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) >= 500)) && ! _POSIX_SOURCE && ! _POSIX_C_SOURCE) && ! (_XOPEN_SOURCE && (_XOPEN_SOURCE - 0) < 500) && ! (_XOPEN_SOURCE && (_XOPEN_SOURCE - 0) < 600) && (_XOPEN_SOURCE && (_XOPEN_SOURCE - 0) < 700)
	public const int _POSIX_C_SOURCE = 200112;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _XOPEN_SOURCE = 700;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _XOPEN_SOURCE_EXTENDED = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _LARGEFILE64_SOURCE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int _ATFILE_SOURCE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ( _ISOC11_SOURCE || ( __STDC_VERSION__ && __STDC_VERSION__ >= 201112L))
	public const int __USE_ISOC11 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ( _ISOC99_SOURCE || _ISOC11_SOURCE || ( __STDC_VERSION__ && __STDC_VERSION__ >= 199901L))
	public const int __USE_ISOC99 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ( _ISOC99_SOURCE || _ISOC11_SOURCE || ( __STDC_VERSION__ && __STDC_VERSION__ >= 199409L))
	public const int __USE_ISOC95 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && __cplusplus && __cplusplus >= 201103L || __GXX_EXPERIMENTAL_CXX0X__
	public const int __USE_ISOCXX11 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _DEFAULT_SOURCE && ! _POSIX_SOURCE && ! _POSIX_C_SOURCE
	public const int __USE_POSIX_IMPLICITLY = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ( _POSIX_SOURCE || ( _POSIX_C_SOURCE && _POSIX_C_SOURCE >= 1) || _XOPEN_SOURCE)
	public const int __USE_POSIX = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _POSIX_C_SOURCE && _POSIX_C_SOURCE >= 2 || _XOPEN_SOURCE
	public const int __USE_POSIX2 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _POSIX_C_SOURCE && (_POSIX_C_SOURCE - 0) >= 199309L
	public const int __USE_POSIX199309 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _POSIX_C_SOURCE && (_POSIX_C_SOURCE - 0) >= 199506L
	public const int __USE_POSIX199506 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _POSIX_C_SOURCE && (_POSIX_C_SOURCE - 0) >= 200112L
	public const int __USE_XOPEN2K = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _POSIX_C_SOURCE && (_POSIX_C_SOURCE - 0) >= 200809L
	public const int __USE_XOPEN2K8 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _XOPEN_SOURCE
	public const int __USE_XOPEN = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) >= 500
	public const int __USE_XOPEN_EXTENDED = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) >= 500
	public const int __USE_UNIX98 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) >= 500
	public const int _LARGEFILE_SOURCE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) >= 500 && (_XOPEN_SOURCE - 0) >= 600 && (_XOPEN_SOURCE - 0) >= 700
	public const int __USE_XOPEN2K8XSI = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _XOPEN_SOURCE && (_XOPEN_SOURCE - 0) >= 500 && (_XOPEN_SOURCE - 0) >= 600
	public const int __USE_XOPEN2KXSI = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _LARGEFILE_SOURCE
	public const int __USE_LARGEFILE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _LARGEFILE64_SOURCE
	public const int __USE_LARGEFILE64 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _FILE_OFFSET_BITS && _FILE_OFFSET_BITS == 64
	public const int __USE_FILE_OFFSET64 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _DEFAULT_SOURCE
	public const int __USE_MISC = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _ATFILE_SOURCE
	public const int __USE_ATFILE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _GNU_SOURCE
	public const int __USE_GNU = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _FORTIFY_SOURCE && _FORTIFY_SOURCE > 0 && ! (! __OPTIMIZE__ || __OPTIMIZE__ <= 0) && ! (!__GNUC_PREREQ (4, 1)) && (_FORTIFY_SOURCE > 1)
	public const int __USE_FORTIFY_LEVEL = 2;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && _FORTIFY_SOURCE && _FORTIFY_SOURCE > 0 && ! (! __OPTIMIZE__ || __OPTIMIZE__ <= 0) && ! (!__GNUC_PREREQ (4, 1)) && ! (_FORTIFY_SOURCE > 1)
	public const int __USE_FORTIFY_LEVEL = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int __USE_FORTIFY_LEVEL = 0;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && __cplusplus ? __cplusplus >= 201402L : __USE_ISOC11
	public const int __GLIBC_USE_DEPRECATED_GETS = 0;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! (__cplusplus ? __cplusplus >= 201402L : __USE_ISOC11)
	public const int __GLIBC_USE_DEPRECATED_GETS = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int _STDC_PREDEF_H = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && __GCC_IEC_559 && __GCC_IEC_559 > 0
	public const int __STDC_IEC_559__ = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && __GCC_IEC_559_COMPLEX && __GCC_IEC_559_COMPLEX > 0
	public const int __STDC_IEC_559_COMPLEX__ = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int __STDC_ISO_10646__ = 201706;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int __GNU_LIBRARY__ = 6;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int __GLIBC__ = 2;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int __GLIBC_MINOR__ = 28;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _XOPEN_SOURCE && (_XOPEN_SOURCE - 0 >= 700)
	public const int __XPG_VISIBLE = 700;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _XOPEN_SOURCE && ! ((_XOPEN_SOURCE - 0 >= 700)) && ((_XOPEN_SOURCE - 0 >= 600))
	public const int __XPG_VISIBLE = 600;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _XOPEN_SOURCE && ! ((_XOPEN_SOURCE - 0 >= 700)) && ! ((_XOPEN_SOURCE - 0 >= 600)) && ((_XOPEN_SOURCE - 0 >= 520))
	public const int __XPG_VISIBLE = 520;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _XOPEN_SOURCE && ! ((_XOPEN_SOURCE - 0 >= 700)) && ! ((_XOPEN_SOURCE - 0 >= 600)) && ! ((_XOPEN_SOURCE - 0 >= 520)) && ((_XOPEN_SOURCE - 0 >= 500))
	public const int __XPG_VISIBLE = 500;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _XOPEN_SOURCE && ! ((_XOPEN_SOURCE - 0 >= 700)) && ! ((_XOPEN_SOURCE - 0 >= 600)) && ! ((_XOPEN_SOURCE - 0 >= 520)) && ! ((_XOPEN_SOURCE - 0 >= 500)) && ((_XOPEN_SOURCE_EXTENDED - 0 == 1))
	public const int __XPG_VISIBLE = 420;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _XOPEN_SOURCE && ! ((_XOPEN_SOURCE - 0 >= 700)) && ! ((_XOPEN_SOURCE - 0 >= 600)) && ! ((_XOPEN_SOURCE - 0 >= 520)) && ! ((_XOPEN_SOURCE - 0 >= 500)) && ! ((_XOPEN_SOURCE_EXTENDED - 0 == 1)) && ((_XOPEN_VERSION - 0 >= 4))
	public const int __XPG_VISIBLE = 400;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _XOPEN_SOURCE && ! ((_XOPEN_SOURCE - 0 >= 700)) && ! ((_XOPEN_SOURCE - 0 >= 600)) && ! ((_XOPEN_SOURCE - 0 >= 520)) && ! ((_XOPEN_SOURCE - 0 >= 500)) && ! ((_XOPEN_SOURCE_EXTENDED - 0 == 1)) && ! ((_XOPEN_VERSION - 0 >= 4))
	public const int __XPG_VISIBLE = 300;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _ANSI_SOURCE && !__POSIX_VISIBLE && !__XPG_VISIBLE
	public const int __XPG_VISIBLE = 0;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _POSIX_C_SOURCE && (_POSIX_C_SOURCE - 0 >= 200809)
	public const int __POSIX_VISIBLE = 200809;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _POSIX_C_SOURCE && ! ((_POSIX_C_SOURCE - 0 >= 200809)) && ((_POSIX_C_SOURCE - 0 >= 200112))
	public const int __POSIX_VISIBLE = 200112;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _POSIX_C_SOURCE && ! ((_POSIX_C_SOURCE - 0 >= 200809)) && ! ((_POSIX_C_SOURCE - 0 >= 200112)) && ((_POSIX_C_SOURCE - 0 >= 199506))
	public const int __POSIX_VISIBLE = 199506;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _POSIX_C_SOURCE && ! ((_POSIX_C_SOURCE - 0 >= 200809)) && ! ((_POSIX_C_SOURCE - 0 >= 200112)) && ! ((_POSIX_C_SOURCE - 0 >= 199506)) && ((_POSIX_C_SOURCE - 0 >= 199309))
	public const int __POSIX_VISIBLE = 199309;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _POSIX_C_SOURCE && ! ((_POSIX_C_SOURCE - 0 >= 200809)) && ! ((_POSIX_C_SOURCE - 0 >= 200112)) && ! ((_POSIX_C_SOURCE - 0 >= 199506)) && ! ((_POSIX_C_SOURCE - 0 >= 199309)) && ((_POSIX_C_SOURCE - 0 >= 2))
	public const int __POSIX_VISIBLE = 199209;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _POSIX_C_SOURCE && ! ((_POSIX_C_SOURCE - 0 >= 200809)) && ! ((_POSIX_C_SOURCE - 0 >= 200112)) && ! ((_POSIX_C_SOURCE - 0 >= 199506)) && ! ((_POSIX_C_SOURCE - 0 >= 199309)) && ! ((_POSIX_C_SOURCE - 0 >= 2))
	public const int __POSIX_VISIBLE = 199009;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && ! _POSIX_C_SOURCE && _POSIX_SOURCE
	public const int __POSIX_VISIBLE = 198808;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _ANSI_SOURCE && !__POSIX_VISIBLE && !__XPG_VISIBLE
	public const int __POSIX_VISIBLE = 0;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _POSIX_C_SOURCE && (_POSIX_C_SOURCE - 0 >= 200809)
	public const int __ISO_C_VISIBLE = 1999;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _POSIX_C_SOURCE && ! ((_POSIX_C_SOURCE - 0 >= 200809)) && ! ((_POSIX_C_SOURCE - 0 >= 200112)) && ((_POSIX_C_SOURCE - 0 >= 199506))
	public const int __ISO_C_VISIBLE = 1990;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && ! _POSIX_C_SOURCE && _POSIX_SOURCE
	public const int __ISO_C_VISIBLE = 0;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && _ISOC11_SOURCE || (__STDC_VERSION__ && __STDC_VERSION__ >= 201112) || (__cplusplus && __cplusplus >= 201703)
	public const int __ISO_C_VISIBLE = 2011;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H && !_BSD_SOURCE && (_ANSI_SOURCE || __XPG_VISIBLE || __POSIX_VISIBLE)
	public const int __BSD_VISIBLE = 0;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __ASSEMBLER__ && ! _SYS_CDEFS_H
	public const int __BSD_VISIBLE = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && __GNUC_PREREQ (2, 7) && __OPTIMIZE__ && ! __OPTIMIZE_SIZE__ && ! __NO_INLINE__ && __extern_inline
	public const int __USE_EXTERN_INLINES = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && __x86_64__
	public const int __WORDSIZE = 64;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && ! __x86_64__
	public const int __WORDSIZE = 32;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && __x86_64__
	public const int __WORDSIZE_COMPAT32 = 1;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int __LITTLE_ENDIAN = 1234;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int __BIG_ENDIAN = 4321;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__
	public const int __PDP_ENDIAN = 3412;
#endif
#if ! RAPIDJSON_ENDIAN && ! __BYTE_ORDER__ && __GLIBC__ && __USE_BSD
	public const int _BITS_BYTESWAP_H = 1;
#endif
#if ! RAPIDJSON_64BIT && __LP64__ || (__x86_64__ && __ILP32__) || _WIN64 || __EMSCRIPTEN__
	public const int RAPIDJSON_64BIT = 1;
#endif
#if ! RAPIDJSON_64BIT && ! (__LP64__ || (__x86_64__ && __ILP32__) || _WIN64 || __EMSCRIPTEN__)
	public const int RAPIDJSON_64BIT = 0;
#endif
#if ! RAPIDJSON_48BITPOINTER_OPTIMIZATION && __amd64__ || __amd64 || __x86_64__ || __x86_64 || _M_X64 || _M_AMD64
	public const int RAPIDJSON_48BITPOINTER_OPTIMIZATION = 1;
#endif
#if ! RAPIDJSON_48BITPOINTER_OPTIMIZATION && ! (__amd64__ || __amd64 || __x86_64__ || __x86_64 || _M_X64 || _M_AMD64)
	public const int RAPIDJSON_48BITPOINTER_OPTIMIZATION = 0;
#endif
#if ! RAPIDJSON_HAS_CXX11_RVALUE_REFS && __clang__ && __has_feature(cxx_rvalue_references) && (_MSC_VER || _LIBCPP_VERSION || __GLIBCXX__ && __GLIBCXX__ >= 20080306)
	public const int RAPIDJSON_HAS_CXX11_RVALUE_REFS = 1;
#endif
#if ! RAPIDJSON_HAS_CXX11_RVALUE_REFS && __clang__ && ! (__has_feature(cxx_rvalue_references) && (_MSC_VER || _LIBCPP_VERSION || __GLIBCXX__ && __GLIBCXX__ >= 20080306))
	public const int RAPIDJSON_HAS_CXX11_RVALUE_REFS = 0;
#endif
#if ! RAPIDJSON_HAS_CXX11_NOEXCEPT && ! __clang__ && ((RAPIDJSON_GNUC && (RAPIDJSON_GNUC >= RAPIDJSON_VERSION_CODE(4,6,0)) && __GXX_EXPERIMENTAL_CXX0X__) || (_MSC_VER && _MSC_VER >= 1900) || (__SUNPRO_CC && __SUNPRO_CC >= 0x5140 && __GXX_EXPERIMENTAL_CXX0X__))
	public const int RAPIDJSON_HAS_CXX11_NOEXCEPT = 1;
#endif
#if ! RAPIDJSON_HAS_CXX11_NOEXCEPT && ! __clang__ && ! ((RAPIDJSON_GNUC && (RAPIDJSON_GNUC >= RAPIDJSON_VERSION_CODE(4,6,0)) && __GXX_EXPERIMENTAL_CXX0X__) || (_MSC_VER && _MSC_VER >= 1900) || (__SUNPRO_CC && __SUNPRO_CC >= 0x5140 && __GXX_EXPERIMENTAL_CXX0X__))
	public const int RAPIDJSON_HAS_CXX11_NOEXCEPT = 0;
#endif
#if ! RAPIDJSON_HAS_CXX11_TYPETRAITS && (_MSC_VER && _MSC_VER >= 1700)
	public const int RAPIDJSON_HAS_CXX11_TYPETRAITS = 1;
#endif
#if ! RAPIDJSON_HAS_CXX11_TYPETRAITS && ! ((_MSC_VER && _MSC_VER >= 1700))
	public const int RAPIDJSON_HAS_CXX11_TYPETRAITS = 0;
#endif
#if ! RAPIDJSON_HAS_CXX11_RANGE_FOR && ! __clang__ && ((RAPIDJSON_GNUC && (RAPIDJSON_GNUC >= RAPIDJSON_VERSION_CODE(4,6,0)) && __GXX_EXPERIMENTAL_CXX0X__) || (_MSC_VER && _MSC_VER >= 1700) || (__SUNPRO_CC && __SUNPRO_CC >= 0x5140 && __GXX_EXPERIMENTAL_CXX0X__))
	public const int RAPIDJSON_HAS_CXX11_RANGE_FOR = 1;
#endif
#if ! RAPIDJSON_HAS_CXX11_RANGE_FOR && ! __clang__ && ! ((RAPIDJSON_GNUC && (RAPIDJSON_GNUC >= RAPIDJSON_VERSION_CODE(4,6,0)) && __GXX_EXPERIMENTAL_CXX0X__) || (_MSC_VER && _MSC_VER >= 1700) || (__SUNPRO_CC && __SUNPRO_CC >= 0x5140 && __GXX_EXPERIMENTAL_CXX0X__))
	public const int RAPIDJSON_HAS_CXX11_RANGE_FOR = 0;
#endif
}